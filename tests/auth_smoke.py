"""Run after dotnet build src/Steward.Server. Uses temporary SQLite and a minimal MQTT test peer."""
import base64
import hashlib
import json
import os
from pathlib import Path
import socket
import sqlite3
import struct
import subprocess
import tempfile
import threading
import time
import urllib.error
import urllib.request

ROOT = Path(__file__).resolve().parents[1]
ASSEMBLY = ROOT / 'src/Steward.Server/bin/Debug/net10.0/Steward.Server.dll'


def password_hash(pin):
    salt = os.urandom(16)
    return base64.b64encode(b'\x01' + struct.pack('>III', 2, 100000, 16) + salt
                            + hashlib.pbkdf2_hmac('sha512', pin.encode(), salt, 100000, 32)).decode()


def mqtt_peer(listener):
    connection, _ = listener.accept()
    with connection:
        def read(count):
            data = b''
            while len(data) < count:
                chunk = connection.recv(count - len(data))
                if not chunk:
                    raise EOFError()
                data += chunk
            return data
        try:
            version = 4
            while True:
                kind = read(1)[0] >> 4
                size, multiplier = 0, 1
                while True:
                    digit = read(1)[0]
                    size += (digit & 127) * multiplier
                    if not digit & 128:
                        break
                    multiplier *= 128
                data = read(size)
                if kind == 1:
                    version = data[6]
                    connection.sendall(b'\x20\x03\x00\x00\x00' if version == 5 else b'\x20\x02\x00\x00')
                elif kind == 8:
                    payload = data[:2] + (b'\x00' if version == 5 else b'') + b'\x00'
                    connection.sendall(bytes([0x90, len(payload)]) + payload)
                elif kind == 12:
                    connection.sendall(b'\xd0\x00')
        except (EOFError, ConnectionError):
            pass


with tempfile.TemporaryDirectory(prefix='steward-auth-') as folder:
    database = str(Path(folder) / 'test.db')
    subprocess.run(['dotnet', 'ef', 'database', 'update', '--no-build', '--project',
                    str(ROOT / 'src/Steward.Server'), '--connection', f'Data Source={database}'],
                   check=True, stdout=subprocess.DEVNULL)
    with sqlite3.connect(database) as db:
        db.execute("INSERT INTO Users (Id, Name, Type, Email, PinHash) VALUES (1, 'Admin', 'Admin', 'admin@example.com', ?)", (password_hash('123456'),))
        db.execute("INSERT INTO Users (Id, Name, Type) VALUES (2, 'Member', 'Member')")
        db.execute("INSERT INTO Users (Id, Name, Type, PinHash) VALUES (3, 'PIN member', 'Member', ?)", (password_hash('654321'),))
    with socket.socket() as broker, socket.socket() as port:
        broker.bind(('127.0.0.1', 0)); broker.listen()
        port.bind(('127.0.0.1', 0)); http_port = port.getsockname()[1]; port.close()
        threading.Thread(target=mqtt_peer, args=(broker,), daemon=True).start()
        env = dict(os.environ, ConnectionStrings__Steward=f'Data Source={database}',
                   ASPNETCORE_URLS=f'http://127.0.0.1:{http_port}',
                   Mqtt__Host='127.0.0.1', Mqtt__Port=str(broker.getsockname()[1]),
                   Logging__LogLevel__Default='Warning')
        with open(Path(folder) / 'server.log', 'w+') as log:
            server = subprocess.Popen(['dotnet', str(ASSEMBLY)], cwd=folder, env=env, stdout=log, stderr=log)
            try:
                def request(path, body=None, cookie=None, header=True):
                    headers = {'Content-Type': 'application/json'}
                    if header:
                        headers['X-Steward-Request'] = '1'
                    if cookie:
                        headers['Cookie'] = cookie
                    req = urllib.request.Request(f'http://127.0.0.1:{http_port}/api{path}',
                                                 data=None if body is None else json.dumps(body).encode(), headers=headers)
                    try:
                        response = urllib.request.urlopen(req, timeout=5)
                    except urllib.error.HTTPError as error:
                        response = error
                    return response.status, response.read(), response.headers
                for _ in range(100):
                    try:
                        if request('/auth/users')[0] == 200:
                            break
                    except (OSError, urllib.error.URLError):
                        time.sleep(.1)
                else:
                    log.seek(0); raise AssertionError(log.read())
                for path in ['/users', '/agents', '/wards', '/policies', '/access/requests', '/auth/session']:
                    assert request(path)[0] == 401, path
                picker = json.loads(request('/auth/users')[1])
                assert all(set(user) == {'id', 'name'} for user in picker)
                assert request('/auth/login', {'userId': 1, 'pin': ''})[0] == 401
                assert request('/auth/login', {'userId': 3, 'pin': ''})[0] == 401
                assert request('/auth/login', {'userId': 2, 'pin': '1'})[0] == 401
                status, body, headers = request('/auth/login', {'userId': 2, 'pin': ''})
                assert status == 200 and json.loads(body)['type'] == 'member'
                member_cookie = headers.get_all('Set-Cookie')[-1].split(';')[0]
                assert 'httponly' in headers.get_all('Set-Cookie')[-1].lower()
                assert 'samesite=strict' in headers.get_all('Set-Cookie')[-1].lower()
                assert request('/auth/session', cookie=member_cookie)[0] == 200
                for path in ['/users', '/agents', '/wards', '/policies', '/access/1']:
                    assert request(path, cookie=member_cookie)[0] == 403, path
                for path, body in [('/access/requests/99/approve', {'userId': 1}),
                                   ('/access/requests/99/reject', {'userId': 1}),
                                   ('/users', {'name': 'Injected', 'deviceIds': []})]:
                    assert request(path, body, member_cookie)[0] == 403, path
                assert request('/access/2', cookie=member_cookie)[0] == 200
                assert request('/access/requests', cookie=member_cookie)[0] == 200
                assert request('/auth/logout', {}, member_cookie, header=False)[0] == 403
                assert request('/auth/logout', {}, member_cookie)[0] == 204
                assert request('/auth/session', cookie=member_cookie)[0] == 401
                status, _, headers = request('/auth/login', {'userId': 1, 'pin': '123456'})
                assert status == 200
                admin_cookie = headers.get_all('Set-Cookie')[-1].split(';')[0]
                assert request('/users', cookie=admin_cookie)[0] == 200
                assert request('/access/2', cookie=admin_cookie)[0] == 200
                status, _, headers = request('/auth/login', {'userId': 3, 'pin': '654321'})
                assert status == 200
                pin_cookie = headers.get_all('Set-Cookie')[-1].split(';')[0]
                with sqlite3.connect(database) as db:
                    db.execute("UPDATE Users SET PinHash = NULL WHERE Id = 3")
                    db.execute("UPDATE Users SET Type = 'Member' WHERE Id = 1")
                assert request('/auth/session', cookie=pin_cookie)[0] == 401
                assert request('/users', cookie=admin_cookie)[0] == 401
                print('PASS: anonymous isolation, minimal picker, admin/member PINs, roles, own access, shared activity, CSRF header, logout replay, PIN/type invalidation.')
            except Exception:
                log.flush(); log.seek(0); print(log.read())
                raise
            finally:
                server.terminate()
                server.wait(timeout=10)
