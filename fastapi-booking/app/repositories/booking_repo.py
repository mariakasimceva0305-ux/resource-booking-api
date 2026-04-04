from datetime import datetime, timedelta

from sqlalchemy import select, func, and_
from sqlalchemy.orm import Session

from app.models import Booking, BookingStatus


class BookingRepository:
    def __init__(self, db: Session):
        self._db = db

    def get_by_id(self, booking_id: str) -> Booking | None:
        return self._db.get(Booking, booking_id)

    def add(self, booking: Booking) -> None:
        self._db.add(booking)

    def count_active_overlaps(
        self,
        resource_id: str,
        start_utc: datetime,
        end_utc: datetime,
        exclude_booking_id: str | None = None,
    ) -> int:
        q = select(func.count()).select_from(Booking).where(
            and_(
                Booking.resource_id == resource_id,
                Booking.status == BookingStatus.active,
                Booking.start_utc < end_utc,
                Booking.end_utc > start_utc,
            )
        )
        if exclude_booking_id:
            q = q.where(Booking.id != exclude_booking_id)
        return self._db.scalar(q) or 0

    def count_active_for_user_on_day(self, user_id: str, day_start_utc: datetime) -> int:
        day_start = day_start_utc.replace(hour=0, minute=0, second=0, microsecond=0)
        day_end = day_start + timedelta(days=1)
        return (
            self._db.scalar(
                select(func.count()).select_from(Booking).where(
                    and_(
                        Booking.user_id == user_id,
                        Booking.status == BookingStatus.active,
                        Booking.start_utc >= day_start,
                        Booking.start_utc < day_end,
                    )
                )
            )
            or 0
        )

    def list_for_user(self, user_id: str) -> list[Booking]:
        return list(
            self._db.scalars(
                select(Booking).where(Booking.user_id == user_id).order_by(Booking.start_utc.desc())
            ).all()
        )

    def count_active(self) -> int:
        return (
            self._db.scalar(
                select(func.count()).select_from(Booking).where(Booking.status == BookingStatus.active)
            )
            or 0
        )

    def count_starts_on_utc_day(self, utc_day: datetime) -> int:
        day_start = utc_day.replace(hour=0, minute=0, second=0, microsecond=0)
        day_end = day_start + timedelta(days=1)
        return (
            self._db.scalar(
                select(func.count()).select_from(Booking).where(
                    and_(
                        Booking.status == BookingStatus.active,
                        Booking.start_utc >= day_start,
                        Booking.start_utc < day_end,
                    )
                )
            )
            or 0
        )
