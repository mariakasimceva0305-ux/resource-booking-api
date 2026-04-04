from datetime import datetime
from pydantic import BaseModel, EmailStr, Field, ConfigDict


class RegisterRequest(BaseModel):
    email: EmailStr
    password: str = Field(min_length=6)
    access_group: str | None = None


class LoginRequest(BaseModel):
    email: EmailStr
    password: str


class AuthResponse(BaseModel):
    token: str
    user_id: str
    email: str
    role: str


class CreateResourceRequest(BaseModel):
    name: str = Field(max_length=200)
    description: str | None = Field(None, max_length=2000)
    required_access_group: str | None = Field(None, max_length=100)


class ResourceDto(BaseModel):
    model_config = ConfigDict(from_attributes=True)

    id: str
    name: str
    description: str | None
    is_active: bool
    required_access_group: str | None


class CreateBookingRequest(BaseModel):
    resource_id: str
    start_utc: datetime
    end_utc: datetime


class UpdateBookingRequest(BaseModel):
    start_utc: datetime
    end_utc: datetime


class BookingDto(BaseModel):
    id: str
    user_id: str
    user_email: str
    resource_id: str
    resource_name: str
    start_utc: datetime
    end_utc: datetime
    status: str


class AdminStatsDto(BaseModel):
    total_users: int
    total_resources: int
    active_bookings: int
    bookings_today: int
