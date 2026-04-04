import uuid
from contextlib import asynccontextmanager
from pathlib import Path

from fastapi import FastAPI
from fastapi.responses import FileResponse, RedirectResponse
from fastapi.staticfiles import StaticFiles
from sqlalchemy import select

from app.database import Base, SessionLocal, engine
from app.models import User, UserRole
from app.routers import admin, auth, bookings, resources
from app.seed_resources import ensure_sample_resources
from app.security import hash_password


@asynccontextmanager
async def lifespan(_app: FastAPI):
    Base.metadata.create_all(bind=engine)
    db = SessionLocal()
    try:
        has_admin = db.scalars(select(User).where(User.role == UserRole.admin).limit(1)).first()
        if not has_admin:
            db.add(
                User(
                    id=str(uuid.uuid4()),
                    email="admin@local",
                    password_hash=hash_password("Admin123!"),
                    role=UserRole.admin,
                    access_group=None,
                )
            )
            db.commit()

        ensure_sample_resources(db)
    finally:
        db.close()
    yield


app = FastAPI(
    title="Сервис бронирования ресурсов",
    description="Веб-интерфейс: откройте /. Документация API: /docs.",
    lifespan=lifespan,
)

_STATIC = Path(__file__).resolve().parent.parent / "static"
if _STATIC.is_dir():
    app.mount("/static", StaticFiles(directory=str(_STATIC)), name="static")

app.include_router(auth.router, prefix="/api")
app.include_router(resources.router, prefix="/api")
app.include_router(bookings.router, prefix="/api")
app.include_router(admin.router, prefix="/api")


@app.get("/")
def spa_index():
    idx = _STATIC / "index.html"
    if idx.is_file():
        return FileResponse(idx)
    return RedirectResponse(url="/docs")
