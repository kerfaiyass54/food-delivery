from datetime import datetime, time
from decimal import Decimal

from sqlalchemy import DateTime, Integer, Numeric, String, Time
from sqlalchemy.orm import Mapped, mapped_column

from app.db.base import Base


class RestaurantRecord(Base):
    __tablename__ = "Restaurants"

    id: Mapped[int] = mapped_column("Id", Integer, primary_key=True)
    name: Mapped[str] = mapped_column("Name", String(200))
    address: Mapped[str] = mapped_column("Address", String(300))
    capacity: Mapped[int] = mapped_column("Capacity", Integer)
    date_of_add: Mapped[datetime] = mapped_column("DateOfAdd", DateTime(timezone=True))
    status: Mapped[int] = mapped_column("Status", Integer)
    time_open: Mapped[time | None] = mapped_column("TimeOpen", Time, nullable=True)
    time_close: Mapped[time | None] = mapped_column("TimeClose", Time, nullable=True)
    nationality: Mapped[str | None] = mapped_column("Nationality", String(100), nullable=True)
    budget: Mapped[Decimal] = mapped_column("Budget", Numeric(18, 2))
    owner_id: Mapped[int] = mapped_column("OwnerId", Integer)
