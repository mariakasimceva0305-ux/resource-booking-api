"""Примеры ресурсов при пустой базе (переговорки, оборудование)."""

from __future__ import annotations

import uuid

from sqlalchemy import func, select
from sqlalchemy.orm import Session

from app.models import Resource

DEFAULT_RESOURCES: list[dict[str, str | None]] = [
    {
        "name": "Переговорная «Сириус»",
        "description": "До 8 человек, флипчарт, экран",
        "required_access_group": None,
    },
    {
        "name": "Переговорная «Вега»",
        "description": "Небольшая комната, до 4 человек",
        "required_access_group": None,
    },
    {
        "name": "Проектор (портативный)",
        "description": "Full HD, сумка и кабели в комплекте",
        "required_access_group": None,
    },
    {
        "name": "Зал для презентаций",
        "description": "До 35 мест; доступ только группе staff",
        "required_access_group": "staff",
    },
    {
        "name": "Ноутбук в аренду",
        "description": "Windows, офис; выдача на сутки",
        "required_access_group": None,
    },
    {
        "name": "Место в open space",
        "description": "Фиксированное рабочее место на день",
        "required_access_group": None,
    },
]


def ensure_sample_resources(db: Session) -> None:
    n = db.scalar(select(func.count()).select_from(Resource)) or 0
    if n > 0:
        return
    for row in DEFAULT_RESOURCES:
        db.add(
            Resource(
                id=str(uuid.uuid4()),
                name=row["name"],
                description=row["description"],
                is_active=True,
                required_access_group=row["required_access_group"],
            )
        )
    db.commit()
