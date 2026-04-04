from sqlalchemy import select, func
from sqlalchemy.orm import Session

from app.models import Resource


class ResourceRepository:
    def __init__(self, db: Session):
        self._db = db

    def get_by_id(self, resource_id: str) -> Resource | None:
        return self._db.get(Resource, resource_id)

    def list_active(self) -> list[Resource]:
        return list(
            self._db.scalars(select(Resource).where(Resource.is_active.is_(True)).order_by(Resource.name)).all()
        )

    def add(self, resource: Resource) -> None:
        self._db.add(resource)

    def count(self) -> int:
        return self._db.scalar(select(func.count()).select_from(Resource)) or 0
