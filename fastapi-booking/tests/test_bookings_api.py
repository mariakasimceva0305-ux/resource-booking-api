from __future__ import annotations

from datetime import datetime, timedelta, timezone

from fastapi.testclient import TestClient


def _user_token(client: TestClient) -> str:
    client.post(
        "/api/auth/register",
        json={"email": "booker@example.com", "password": "secret12", "access_group": None},
    )
    r = client.post(
        "/api/auth/login",
        json={"email": "booker@example.com", "password": "secret12"},
    )
    assert r.status_code == 200
    return r.json()["token"]


def test_list_resources_and_create_booking(client: TestClient) -> None:
    token = _user_token(client)
    headers = {"Authorization": f"Bearer {token}"}
    lr = client.get("/api/resources", headers=headers)
    assert lr.status_code == 200
    resources = lr.json()
    assert len(resources) >= 1
    rid = resources[0]["id"]

    start = datetime.now(timezone.utc) + timedelta(days=2)
    start = start.replace(hour=10, minute=0, second=0, microsecond=0)
    end = start + timedelta(hours=1)

    br = client.post(
        "/api/bookings",
        headers=headers,
        json={
            "resource_id": rid,
            "start_utc": start.isoformat().replace("+00:00", "Z"),
            "end_utc": end.isoformat().replace("+00:00", "Z"),
        },
    )
    assert br.status_code == 200
    body = br.json()
    assert body["resource_id"] == rid
    assert body["status"] == "Active"

    mine = client.get("/api/bookings/mine", headers=headers)
    assert mine.status_code == 200
    assert len(mine.json()) >= 1
