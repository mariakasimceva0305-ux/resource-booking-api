from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session

from app.database import get_db
from app.deps import get_current_user, require_admin
from app.models import User
from app.schemas import CreateResourceRequest, ResourceDto
from app.services.resource_service import ResourceService

router = APIRouter(prefix="/resources", tags=["resources"])


@router.get("", response_model=list[ResourceDto])
def list_resources(
    db: Session = Depends(get_db),
    user: User = Depends(get_current_user),
) -> list[ResourceDto]:
    return ResourceService(db).list_visible(user)


@router.post("", response_model=ResourceDto)
def create_resource(
    req: CreateResourceRequest,
    db: Session = Depends(get_db),
    _: User = Depends(require_admin),
) -> ResourceDto:
    return ResourceService(db).create(req)
