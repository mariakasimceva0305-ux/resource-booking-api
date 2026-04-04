import uuid

from sqlalchemy.orm import Session

from app.models import Resource, UserRole, User
from app.repositories.resource_repo import ResourceRepository
from app.schemas import CreateResourceRequest, ResourceDto


class ResourceService:
    def __init__(self, db: Session):
        self._db = db
        self._resources = ResourceRepository(db)

    def create(self, req: CreateResourceRequest) -> ResourceDto:
        r = Resource(
            id=str(uuid.uuid4()),
            name=req.name.strip(),
            description=req.description.strip() if req.description else None,
            is_active=True,
            required_access_group=(
                req.required_access_group.strip() if req.required_access_group and req.required_access_group.strip() else None
            ),
        )
        self._resources.add(r)
        self._db.commit()
        self._db.refresh(r)
        return ResourceDto.model_validate(r)

    def list_visible(self, viewer: User) -> list[ResourceDto]:
        all_active = self._resources.list_active()
        if viewer.role == UserRole.admin:
            return [ResourceDto.model_validate(x) for x in all_active]
        ag = viewer.access_group
        out: list[Resource] = []
        for r in all_active:
            if not r.required_access_group:
                out.append(r)
            elif ag and r.required_access_group.lower() == ag.lower():
                out.append(r)
        return [ResourceDto.model_validate(x) for x in out]
