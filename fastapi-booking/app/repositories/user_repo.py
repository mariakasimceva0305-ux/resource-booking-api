from sqlalchemy import select, func
from sqlalchemy.orm import Session

from app.models import User


class UserRepository:
    def __init__(self, db: Session):
        self._db = db

    def get_by_id(self, user_id: str) -> User | None:
        return self._db.get(User, user_id)

    def get_by_email(self, email: str) -> User | None:
        return self._db.scalars(select(User).where(User.email == email)).first()

    def add(self, user: User) -> None:
        self._db.add(user)

    def count(self) -> int:
        return self._db.scalar(select(func.count()).select_from(User)) or 0
