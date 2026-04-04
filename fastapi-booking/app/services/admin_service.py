from datetime import datetime, timezone

from sqlalchemy.orm import Session

from app.repositories.booking_repo import BookingRepository
from app.repositories.resource_repo import ResourceRepository
from app.repositories.user_repo import UserRepository
from app.schemas import AdminStatsDto


class AdminService:
    def __init__(self, db: Session):
        self._users = UserRepository(db)
        self._resources = ResourceRepository(db)
        self._bookings = BookingRepository(db)

    def stats(self) -> AdminStatsDto:
        today = datetime.now(timezone.utc).replace(tzinfo=None).replace(hour=0, minute=0, second=0, microsecond=0)
        return AdminStatsDto(
            total_users=self._users.count(),
            total_resources=self._resources.count(),
            active_bookings=self._bookings.count_active(),
            bookings_today=self._bookings.count_starts_on_utc_day(today),
        )
