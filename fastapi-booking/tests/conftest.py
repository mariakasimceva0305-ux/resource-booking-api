from __future__ import annotations

import os
import tempfile
import uuid
from pathlib import Path

import pytest
from fastapi.testclient import TestClient

_db_file = Path(tempfile.gettempdir()) / f"booking_api_test_{uuid.uuid4().hex}.db"
if _db_file.exists():
    _db_file.unlink()

os.environ["DATABASE_URL"] = f"sqlite:///{_db_file.resolve().as_posix()}"
os.environ["BOOTSTRAP_ADMIN_ENABLED"] = "true"
os.environ["BOOTSTRAP_ADMIN_EMAIL"] = "admin@example.com"
os.environ["BOOTSTRAP_ADMIN_PASSWORD"] = "TestAdminSecret123"
os.environ["JWT_SECRET"] = "UnitTestJwtSecretKeyMustBe32Chars!!"

from app.main import app  # noqa: E402


@pytest.fixture()
def client() -> TestClient:
    with TestClient(app) as c:
        yield c
