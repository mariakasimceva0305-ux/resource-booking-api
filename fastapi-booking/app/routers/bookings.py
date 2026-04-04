from fastapi import APIRouter, Depends, Response, status
from sqlalchemy.orm import Session

from app.database import get_db
from app.deps import get_current_user
from app.models import User
from app.schemas import BookingDto, CreateBookingRequest, UpdateBookingRequest
from app.services.booking_service import BookingAppService

router = APIRouter(prefix="/bookings", tags=["bookings"])


@router.get("/mine", response_model=list[BookingDto])
def mine(
    db: Session = Depends(get_db),
    user: User = Depends(get_current_user),
) -> list[BookingDto]:
    return BookingAppService(db).list_mine(user)


@router.post("", response_model=BookingDto)
def create_booking(
    req: CreateBookingRequest,
    db: Session = Depends(get_db),
    user: User = Depends(get_current_user),
) -> BookingDto:
    return BookingAppService(db).create(user, req)


@router.put("/{booking_id}", response_model=BookingDto)
def update_booking(
    booking_id: str,
    req: UpdateBookingRequest,
    db: Session = Depends(get_db),
    user: User = Depends(get_current_user),
) -> BookingDto:
    return BookingAppService(db).update(user, booking_id, req)


@router.post("/{booking_id}/cancel", status_code=status.HTTP_204_NO_CONTENT, response_class=Response)
def cancel_booking(
    booking_id: str,
    db: Session = Depends(get_db),
    user: User = Depends(get_current_user),
) -> Response:
    BookingAppService(db).cancel(user, booking_id)
    return Response(status_code=status.HTTP_204_NO_CONTENT)
