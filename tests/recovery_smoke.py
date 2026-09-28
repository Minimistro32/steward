"""Admin PIN recovery integration tests. All mail stays in a local SMTP test peer."""
from contextlib import contextmanager
from email import policy
from email.parser import BytesParser
import json
import os
from pathlib import Path
import re
import socket
import sqlite3
import subprocess
import tempfile
import threading
import time
import urllib.error
import urllib.request
from auth_smoke import ASSEMBLY, ROOT, mqtt_peer, password_hash

messages = []
smtp_failure = False


def smtp_peer(listener):
    while True:
        try:
            connection, _ = listener.accept()
        except OSError:
            return
        with connection, connection.makefile('rb') as stream:
            connection.sendall(b'220 localhost SMTP test peer\r\n')
            while line := stream.readline():
                command = line.upper()
                if command.startswith((b'EHLO', b'HELO')):
                    connection.sendall(b'250 localhost\r\n')
                elif command.startswith(b'DATA'):
                    connection.sendall(b'354 Send message\r\n')
                    body = b''
                    while (line := stream.readline()) != b'.\r\n':
                        if not line:
                            break
                        body += line
                    messages.append(body)
                    connection.sendall(b'250 Accepted\r\n')
                elif command.startswith(b'QUIT'):
                    connection.sendall(b'221 Bye\r\n')
                    break
                elif command.startswith(b'MAIL') and smtp_failure:
                    connection.sendall(b'550 Test failure\r\n')
                else:
                    connection.sendall(b'250 OK\r\n')


@contextmanager
def server(folder, database, smtp_port, enabled=True):
    with socket.socket() as broker, socket.socket() as port:
        broker.bind(('127.0.0.1', 0)); broker.listen()
        threading.Thread(target=mqtt_peer, args=(broker,), daemon=True).start()
        port.bind(('127.0.0.1', 0)); http_port = port.getsockname()[1]; port.close()
        env = dict(os.environ, ConnectionStrings__Steward=f'Data Source={database}',
                   ASPNETCORE_URLS=f'http://127.0.0.1:{http_port}',
                   Mqtt__Host='127.0.0.1', Mqtt__Port=str(broker.getsockname()[1]),
                   Logging__LogLevel__Default='Warning', Smtp__Enabled=str(enabled),
                   Smtp__Host='127.0.0.1', Smtp__Port=str(smtp_port),
                   Smtp__UseStartTls='false', Smtp__Username='', Smtp__Password='', Smtp__FromAddress='steward@example.com')
        with open(Path(folder) / 'server.log', 'w+') as log:
            process = subprocess.Popen(['dotnet', str(ASSEMBLY)], cwd=folder, env=env, stdout=log, stderr=log)
            def request(path, body=None, cookie=None, method=None):
                headers = {'Content-Type': 'application/json', 'X-Steward-Request': '1'}
                if cookie:
                    headers['Cookie'] = cookie
                req = urllib.request.Request(f'http://127.0.0.1:{http_port}/api{path}',
                    data=None if body is None else json.dumps(body).encode(), headers=headers, method=method)
                try:
                    response = urllib.request.urlopen(req, timeout=10)
                except urllib.error.HTTPError as error:
                    response = error
                return response.status, response.read(), response.headers
            try:
                for _ in range(100):
                    try:
                        if request('/auth/recovery')[0] == 200:
                            break
                    except OSError:
                        time.sleep(.1)
                else:
                    raise AssertionError('Server did not start')
                yield request
            except Exception:
                log.flush(); log.seek(0); print(log.read())
                raise
            finally:
                process.terminate(); process.wait(timeout=10)


with tempfile.TemporaryDirectory(prefix='steward-recovery-') as folder, socket.socket() as smtp:
    database = str(Path(folder) / 'test.db')
    subprocess.run(['dotnet', 'ef', 'database', 'update', '--no-build', '--project',
        str(ROOT / 'src/Steward.Server'), '--connection', f'Data Source={database}'], check=True, stdout=subprocess.DEVNULL)
    with sqlite3.connect(database) as db:
        db.execute("INSERT INTO Users (Id, Name, Type, Email, PinHash) VALUES (1, 'Admin', 'Admin', 'admin@example.com', ?)", (password_hash('1234'),))
        db.execute("INSERT INTO Users (Id, Name, Type, Email) VALUES (2, 'Member', 'Member', 'member@example.com')")
    smtp.bind(('127.0.0.1', 0)); smtp.listen()
    threading.Thread(target=smtp_peer, args=(smtp,), daemon=True).start()
    reset = {'userId': 1, 'email': 'admin@example.com'}
    with server(folder, database, smtp.getsockname()[1]) as request:
        assert json.loads(request('/auth/recovery')[1]) == {'enabled': True}
        assert request('/auth/recovery', {'userId': 1, 'email': 'wrong@example.com'})[0] == 202
        assert request('/auth/recovery', {'userId': 2, 'email': 'member@example.com'})[0] == 202
        assert not messages
        old_login = request('/auth/login', {'userId': 1, 'pin': '1234'})
        assert old_login[0] == 200
        old_cookie = old_login[2].get_all('Set-Cookie')[-1].split(';')[0]
        assert request('/auth/recovery', reset)[0] == 202
        assert len(messages) == 1
        message = BytesParser(policy=policy.default).parsebytes(messages[0])
        assert message['To'] == 'admin@example.com'
        new_pin = re.search(r'PIN is: (\d{8})', message.get_content()).group(1)
        with sqlite3.connect(database) as db:
            row = db.execute('SELECT PinHash, RecoveryPinHash FROM Users WHERE Id=1').fetchone()
            assert row[1] and row[1] != new_pin and row[0] != row[1]
        assert request('/auth/recovery', reset)[0] == 202
        assert len(messages) == 1  # Account cooldown.
        assert request('/auth/session', cookie=old_cookie)[0] == 200
        assert request('/auth/login', {'userId': 1, 'pin': '1234'})[0] == 200
        new_login = request('/auth/login', {'userId': 1, 'pin': new_pin})
        assert new_login[0] == 200
        cookie = new_login[2].get_all('Set-Cookie')[-1].split(';')[0]
        assert request('/auth/session', cookie=old_cookie)[0] == 401
        assert request('/auth/login', {'userId': 1, 'pin': '1234'})[0] == 401
        with sqlite3.connect(database) as db:
            assert db.execute('SELECT RecoveryPinHash FROM Users WHERE Id=1').fetchone() == (None,)
            db.execute("UPDATE Users SET RecoveryPinHash=?, RecoveryPinExpiresAt='2000-01-01 00:00:00+00:00' WHERE Id=1", (password_hash('00009999'),))
        assert request('/auth/login', {'userId': 1, 'pin': '00009999'})[0] == 401
        assert request('/auth/login', {'userId': 1, 'pin': new_pin})[0] == 200
        assert request('/users/1', {'name': 'Admin', 'type': 'admin', 'email': 'admin@example.com', 'pin': '5678'}, cookie, 'PUT')[0] == 200
        with sqlite3.connect(database) as db:
            assert db.execute('SELECT RecoveryPinHash FROM Users WHERE Id=1').fetchone() == (None,)
            db.execute('UPDATE Users SET LastRecoveryEmailAt=NULL WHERE Id=1')
        assert request('/auth/recovery', {'userId': 99, 'email': 'nobody@example.com'})[0] == 202
        assert request('/auth/recovery', reset)[0] == 429
    smtp_failure = True
    with server(folder, database, smtp.getsockname()[1]) as request:
        assert request('/auth/recovery', reset)[0] == 503
        assert len(messages) == 1
        with sqlite3.connect(database) as db:
            assert db.execute('SELECT RecoveryPinHash, LastRecoveryEmailAt FROM Users WHERE Id=1').fetchone() == (None, None)
        assert request('/auth/login', {'userId': 1, 'pin': '5678'})[0] == 200
    with server(folder, database, smtp.getsockname()[1], enabled=False) as request:
        assert json.loads(request('/auth/recovery')[1]) == {'enabled': False}
        assert request('/auth/recovery', reset)[0] == 503
    print('PASS: SMTP delivery, admin-only matching, hashed PIN, cooldown/rate limits, expiry, activation, session revocation, manual-change cancellation, SMTP failure rollback, and disabled configuration.')
