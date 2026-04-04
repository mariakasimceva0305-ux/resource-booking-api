import uuid

from fastapi import HTTPException
from sqlalchemy.orm import Session

from app.models import User, UserRole
from app.repositories.user_repo import UserRepository
from app.schemas import RegisterRequest, LoginRequest, AuthResponse
from app.security import hash_password, verify_password, create_access_token


class AuthService:
    def __init__(self, db: Session):
        self._db = db
        self._users = UserRepository(db)

    def register(self, req: RegisterRequest) -> AuthResponse:
        email = req.email.strip().lower()
        if self._users.get_by_email(email):
            raise HTTPException(status_code=409, detail="Пользователь с таким email уже зарегистрирован.")

        user = User(
            id=str(uuid.uuid4()),
            email=email,
            password_hash=hash_password(req.password),
            role=UserRole.user,
            access_group=(req.access_group.strip() if req.access_group and req.access_group.strip() else None),
        )
        self._users.add(user)
        self._db.commit()
        self._db.refresh(user)
        token = create_access_token(
            user.id,
            {"email": user.email, "role": user.role.value, "access_group": user.access_group or ""},
        )
        return AuthResponse(token=token, user_id=user.id, email=user.email, role=user.role.value)

    def login(self, req: LoginRequest) -> AuthResponse:
        email = req.email.strip().lower()
        user = self._users.get_by_email(email)
        if not user or not verify_password(req.password, user.password_hash):
            raise HTTPException(status_code=401, detail="Неверный email или пароль.")

        token = create_access_token(
            user.id,
            {"email": user.email, "role": user.role.value, "access_group": user.access_group or ""},
        )
        return AuthResponse(token=token, user_id=user.id, email=user.email, role=user.role.value)
