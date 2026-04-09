from __future__ import annotations

from fastapi.testclient import TestClient


def test_login_bootstrap_admin(client: TestClient) -> None:
    r = client.post(
        "/api/auth/login",
        json={"email": "admin@example.com", "password": "TestAdminSecret123"},
    )
    assert r.status_code == 200
    data = r.json()
    assert "token" in data
    assert data["email"] == "admin@example.com"
    assert data["role"] == "Admin"


def test_register_and_login(client: TestClient) -> None:
    r = client.post(
        "/api/auth/register",
        json={"email": "user1@example.com", "password": "secret12", "access_group": None},
    )
    assert r.status_code == 200
    reg = r.json()
    assert reg["email"] == "user1@example.com"

    r2 = client.post(
        "/api/auth/login",
        json={"email": "user1@example.com", "password": "secret12"},
    )
    assert r2.status_code == 200
    assert r2.json()["token"]
