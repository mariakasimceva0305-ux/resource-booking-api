import uuid
from datetime import datetime, timezone

from fastapi import HTTPException
from sqlalchemy.orm import Session

from app.config import settings
from app.models import Booking, BookingStatus, User, UserRole
from app.repositories.booking_repo import BookingRepository
from app.repositories.resource_repo import ResourceRepository
from app.repositories.user_repo import UserRepository
from app.schemas import BookingDto, CreateBookingRequest, UpdateBookingRequest


def _normalize_utc(dt: datetime) -> datetime:
    if dt.tzinfo is not None:
        return dt.astimezone(timezone.utc).replace(tzinfo=None)
    return dt


class BookingAppService:
    def __init__(self, db: Session):
        self._db = db
        self._bookings = BookingRepository(db)
        self._resources = ResourceRepository(db)
        self._users = UserRepository(db)

    def create(self, user: User, req: CreateBookingRequest) -> BookingDto:
        start_utc = _normalize_utc(req.start_utc)
        end_utc = _normalize_utc(req.end_utc)
        if end_utc <= start_utc:
            raise HTTPException(status_code=400, detail="Время окончания должно быть позже начала.")

        resource = self._resources.get_by_id(req.resource_id)
        if not resource:
            raise HTTPException(status_code=404, detail="Ресурс не найден.")
        if not resource.is_active:
            raise HTTPException(status_code=400, detail="Ресурс недоступен для бронирования.")

        if resource.required_access_group:
            if not user.access_group or resource.required_access_group.lower() != user.access_group.lower():
                raise HTTPException(status_code=403, detail="Нет доступа к этому ресурсу.")

        day_start = start_utc.replace(hour=0, minute=0, second=0, microsecond=0)
        if self._bookings.count_active_for_user_on_day(user.id, day_start) >= settings.max_bookings_per_day:
            raise HTTPException(
                status_code=400,
                detail=f"Превышен лимит активных бронирований в день ({settings.max_bookings_per_day}).",
            )

        if self._bookings.count_active_overlaps(resource.id, start_utc, end_utc, None) > 0:
            raise HTTPException(status_code=409, detail="Интервал пересекается с существующей бронью.")

        b = Booking(
            id=str(uuid.uuid4()),
            user_id=user.id,
            resource_id=resource.id,
            start_utc=start_utc,
            end_utc=end_utc,
            status=BookingStatus.active,
        )
        self._bookings.add(b)
        self._db.commit()
        self._db.refresh(b)
        return self._to_dto(b)

    def update(self, user: User, booking_id: str, req: UpdateBookingRequest) -> BookingDto:
        start_utc = _normalize_utc(req.start_utc)
        end_utc = _normalize_utc(req.end_utc)
        if end_utc <= start_utc:
            raise HTTPException(status_code=400, detail="Время окончания должно быть позже начала.")

        b = self._bookings.get_by_id(booking_id)
        if not b:
            raise HTTPException(status_code=404, detail="Бронирование не найдено.")
        if b.status != BookingStatus.active:
            raise HTTPException(status_code=400, detail="Нельзя изменить отменённое бронирование.")
        if user.role != UserRole.admin and b.user_id != user.id:
            raise HTTPException(status_code=403, detail="Нет прав на изменение этой брони.")

        if self._bookings.count_active_overlaps(b.resource_id, start_utc, end_utc, b.id) > 0:
            raise HTTPException(status_code=409, detail="Новый интервал пересекается с другой бронью.")

        b.start_utc = start_utc
        b.end_utc = end_utc
        self._db.commit()
        self._db.refresh(b)
        return self._to_dto(b)

    def cancel(self, user: User, booking_id: str) -> None:
        b = self._bookings.get_by_id(booking_id)
        if not b:
            raise HTTPException(status_code=404, detail="Бронирование не найдено.")
        if user.role != UserRole.admin and b.user_id != user.id:
            raise HTTPException(status_code=403, detail="Нет прав на отмену этой брони.")
        if b.status != BookingStatus.active:
            return
        b.status = BookingStatus.cancelled
        self._db.commit()

    def list_mine(self, user: User) -> list[BookingDto]:
        return [self._to_dto(b) for b in self._bookings.list_for_user(user.id)]

    def _to_dto(self, b: Booking) -> BookingDto:
        u = self._users.get_by_id(b.user_id)
        r = self._resources.get_by_id(b.resource_id)
        return BookingDto(
            id=b.id,
            user_id=b.user_id,
            user_email=u.email if u else "",
            resource_id=b.resource_id,
            resource_name=r.name if r else "",
            start_utc=b.start_utc,
            end_utc=b.end_utc,
            status=b.status.value,
        )
