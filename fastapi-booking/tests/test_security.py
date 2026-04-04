from app.security import hash_password, verify_password


def test_password_hash_roundtrip():
    h = hash_password("SecretPass123")
    assert verify_password("SecretPass123", h) is True
    assert verify_password("wrong", h) is False
