from fastapi import APIRouter, Depends
from sqlalchemy.orm import Session

from app.database import get_db
from app.deps import require_admin
from app.models import User
from app.schemas import AdminStatsDto
from app.services.admin_service import AdminService

router = APIRouter(prefix="/admin", tags=["admin"])


@router.get("/stats", response_model=AdminStatsDto)
def stats(
    db: Session = Depends(get_db),
    _: User = Depends(require_admin),
) -> AdminStatsDto:
    return AdminService(db).stats()
