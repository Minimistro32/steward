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


def main():
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
                    def request(path, body=None, cookie=None, header=True, method=None):
                        headers = {'Content-Type': 'application/json'}
                        if header:
                            headers['X-Steward-Request'] = '1'
                        if cookie:
                            headers['Cookie'] = cookie
                        req = urllib.request.Request(f'http://127.0.0.1:{http_port}/api{path}',
                                                     data=None if body is None else json.dumps(body).encode(), headers=headers, method=method)
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
                    assert all(set(user) == {'id', 'name', 'type'} for user in picker)
                    assert {user['id']: user['type'] for user in picker} == {1: 'admin', 2: 'member', 3: 'member'}
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
                    # Configurable override requirements and approval reasons.
                    with sqlite3.connect(database) as db:
                        db.execute("INSERT INTO Wards (Id, Name, Tags) VALUES (1, 'Test ward', '[]')")
                        db.execute("INSERT INTO WardUsers (WardId, UserId) VALUES (1, 2)")
                    policy_body = {'name': 'Override test', 'wardId': 1, 'tags': [],
                        'schedule': {'days': list(range(7)), 'startTime': '', 'endTime': ''},
                        'access': {'dailyTimeMinutes': 0},
                        'override': {'allowed': True, 'requirement': 'delay', 'delayMinutes': 2, 'randomTextLength': 75, 'allowance': {}}}
                    status, body, _ = request('/policies', policy_body, admin_cookie)
                    assert status == 201
                    policy_id = json.loads(body)['id']
                    policy_path = f'/policies/{policy_id}'
                    assert json.loads(request(policy_path, cookie=admin_cookie)[1])['override']['delayMinutes'] == 2
                    request_body = {'policyId': policy_id, 'requestedMinutes': 1}
                    status, body, _ = request('/access/2/override', request_body, admin_cookie)
                    pending = json.loads(body)
                    assert status == 200 and pending['requirement'] == 'delay'
                    from datetime import datetime, timezone
                    wait = (datetime.fromisoformat(pending['availableAt']) - datetime.now(timezone.utc)).total_seconds()
                    assert 115 < wait <= 120
                    assert json.loads(request(f"/access/requests/{pending['overrideRequestId']}/complete", {'userId': 2}, admin_cookie)[1])['state'] == 'pending'
                    policy_body['override']['requirement'] = 'randomText'
                    assert request(policy_path, policy_body, admin_cookie, method='PUT')[0] == 204
                    status, body, _ = request('/access/2/override', request_body, admin_cookie)
                    random_request = json.loads(body)
                    assert status == 200 and len(random_request['challengeText']) >= 75
                    assert len(random_request['challengeText']) < 100
                    policy_body['override']['randomTextLength'] = 0
                    assert request(policy_path, policy_body, admin_cookie, method='PUT')[0] == 400
                    policy_body['override']['randomTextLength'] = 75
                    policy_body['override']['requirement'] = 'userApproval'
                    assert request(policy_path, policy_body, admin_cookie, method='PUT')[0] == 204
                    normal_attempt = json.loads(request('/access/2/request', request_body, admin_cookie)[1])
                    assert normal_attempt['state'] == 'overrideRequired' and normal_attempt['requirement'] == 'userApproval'
                    options = json.loads(request('/access/2', cookie=admin_cookie)[1])['options']
                    assert next(option for option in options if option['policyId'] == policy_id)['requirement'] == 'userApproval'
                    for reason in [None, '  ', 'x' * 2001]:
                        assert request('/access/2/override', dict(request_body, reason=reason), admin_cookie)[0] == 400
                    status, body, _ = request('/access/2/override', dict(request_body, reason='  Finish my homework  '), admin_cookie)
                    assert status == 200
                    approval_id = json.loads(body)['overrideRequestId']
                    activity = json.loads(request('/access/requests', cookie=admin_cookie)[1])
                    assert next(item for item in activity if item['id'] == approval_id)['reason'] == 'Finish my homework'
                    assert request('/access/2/override', dict(request_body, reason='Updated reason'), admin_cookie)[0] == 200
                    assert request(f'/access/requests/{approval_id}/reject', {'userId': 1}, admin_cookie)[0] == 200
                    activity = json.loads(request('/access/requests', cookie=admin_cookie)[1])
                    assert next(item for item in activity if item['id'] == approval_id)['reason'] == 'Updated reason'
                    # Account creation/editing: validation, credential preservation, and device assignments.
                    for body in [{'name': ' '}, {'name': 'Bad admin', 'type': 'admin'},
                                 {'name': 'Bad email', 'email': 'invalid'}, {'name': 'Bad PIN', 'pin': '123'}, {'name': 'Non-numeric PIN', 'pin': '12a4'},
                                 {'name': 'Too long', 'pin': '1' * 129}]:
                        assert request('/users', body, admin_cookie)[0] == 400
                    assert request('/users/1', {'name': 'Admin', 'type': 'member'}, admin_cookie, method='PUT')[0] == 400
                    assert request('/users/1', cookie=admin_cookie, method='DELETE')[0] == 409
                    status, body, _ = request('/users', {'name': ' New member ', 'type': 'member'}, admin_cookie)
                    created = json.loads(body)
                    assert status == 201 and created['name'] == 'New member' and not created['hasPin']
                    user_path = f"/users/{created['id']}"
                    assert 'pinHash' not in created and 'pin' not in created
                    with sqlite3.connect(database) as db:
                        db.execute("INSERT INTO Agents (Id, InstanceId, Version, Name) VALUES ('test', 'test', '1', 'Test')")
                        db.execute("INSERT INTO Devices (Id, DeviceId, Name, AgentId) VALUES (1, 'test', 'Device', 'test')")
                        db.execute("INSERT INTO UserDevices (UserId, DeviceId) VALUES (?, 1)", (created['id'],))
                    status, body, _ = request(user_path, {'name': 'Renamed', 'type': 'member', 'pin': '112233'}, admin_cookie, method='PUT')
                    assert status == 200 and json.loads(body)['deviceIds'] == [1] and json.loads(body)['hasPin']
                    with sqlite3.connect(database) as db:
                        saved_hash = db.execute('SELECT PinHash FROM Users WHERE Id = ?', (created['id'],)).fetchone()[0]
                        assert saved_hash != '112233'
                    assert request(user_path, {'name': 'Again', 'type': 'member', 'pin': ''}, admin_cookie, method='PUT')[0] == 200
                    with sqlite3.connect(database) as db:
                        assert db.execute('SELECT PinHash FROM Users WHERE Id = ?', (created['id'],)).fetchone()[0] == saved_hash
                    assert request(user_path, {'name': 'Again', 'type': 'member', 'clearPin': True}, admin_cookie, method='PUT')[0] == 200
                    assert not json.loads(request(user_path, cookie=admin_cookie)[1])['hasPin']
                    status, body, _ = request('/users', {'name': 'Second admin', 'type': 'admin', 'email': 'second@example.com', 'pin': '445566'}, admin_cookie)
                    assert status == 201 and json.loads(body)['hasPin']
                    other_admin_path = f"/users/{json.loads(body)['id']}"
                    assert request(other_admin_path, cookie=admin_cookie)[0] == 403
                    for update in [{'name': 'Changed', 'type': 'admin', 'email': 'other@example.com', 'pin': '1234'},
                                   {'name': 'Demoted', 'type': 'member', 'clearPin': True}]:
                        assert request(other_admin_path, update, admin_cookie, method='PUT')[0] == 403
                    assert request(other_admin_path, cookie=admin_cookie, method='DELETE')[0] == 403
                    assert request(other_admin_path + '/devices/1', {}, admin_cookie, method='PUT')[0] == 403
                    assert request(other_admin_path + '/devices/1', cookie=admin_cookie, method='DELETE')[0] == 403
                    assert request('/users/1', {'name': 'My edited name', 'type': 'admin', 'email': 'admin@example.com'}, admin_cookie, method='PUT')[0] == 200
                    for pin in ['0123', '1234567']:
                        status, body, _ = request('/users', {'name': 'PIN length test', 'type': 'member', 'pin': pin}, admin_cookie)
                        assert status == 201
                        assert json.loads(body)['hasPin']
                    # A previously PIN-free member must stop accepting an empty PIN.
                    assert request('/users/2', {'name': 'Member', 'type': 'member', 'pin': '0123'}, admin_cookie, method='PUT')[0] == 200
                    assert request('/auth/login', {'userId': 2, 'pin': ''})[0] == 401
                    assert request('/auth/login', {'userId': 2, 'pin': '0123'})[0] == 200
                    assert request('/users/99999', {'name': 'Missing'}, admin_cookie, method='PUT')[0] == 404

                    status, _, headers = request('/auth/login', {'userId': 3, 'pin': '654321'})
                    assert status == 200
                    pin_cookie = headers.get_all('Set-Cookie')[-1].split(';')[0]
                    # Reset another member's existing PIN through the API, then prove
                    # their old session and PIN fail while the new PIN succeeds.
                    assert request('/users/3', {'name': 'PIN member', 'type': 'member', 'pin': '0123'}, admin_cookie, method='PUT')[0] == 200
                    assert request('/auth/session', cookie=pin_cookie)[0] == 401
                    assert request('/auth/login', {'userId': 3, 'pin': '654321'})[0] == 401
                    assert request('/auth/login', {'userId': 3, 'pin': '0123'})[0] == 200
                    assert request('/auth/session', cookie=admin_cookie)[0] == 200
                    with sqlite3.connect(database) as db:
                        db.execute("UPDATE Users SET PinHash = NULL WHERE Id = 3")
                        db.execute("UPDATE Users SET Type = 'Member' WHERE Id = 1")
                    assert request('/auth/session', cookie=pin_cookie)[0] == 401
                    assert request('/users', cookie=admin_cookie)[0] == 401
                    print('PASS: anonymous isolation, minimal picker, admin/member PINs, roles, own access, shared activity, CSRF header, logout replay, PIN/type invalidation, user create/edit validation, PIN preservation/clearing, devices, last-admin protection, configured override requirements, and approval reasons.')
                except Exception:
                    log.flush(); log.seek(0); print(log.read())
                    raise
                finally:
                    server.terminate()
                    server.wait(timeout=10)


if __name__ == "__main__":
    main()
