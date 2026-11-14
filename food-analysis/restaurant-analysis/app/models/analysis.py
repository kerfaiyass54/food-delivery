from datetime import datetime
from typing import Any

from sqlalchemy import DateTime, ForeignKey, Index, Integer, JSON, String, func
from sqlalchemy.dialects.postgresql import JSONB
from sqlalchemy.orm import Mapped, mapped_column

from app.db.base import Base


class RestaurantAnalysis(Base):
    __tablename__ = "RestaurantAnalyses"
    __table_args__ = (
        Index("IX_RestaurantAnalyses_RestaurantId_CreatedAt", "RestaurantId", "CreatedAt"),
    )

    id: Mapped[int] = mapped_column("Id", Integer, primary_key=True, autoincrement=True)
    restaurant_id: Mapped[int] = mapped_column(
        "RestaurantId",
        ForeignKey("Restaurants.Id", ondelete="CASCADE"),
        nullable=False,
    )
    restaurant_name: Mapped[str] = mapped_column("RestaurantName", String(200), nullable=False)
    analysis_type: Mapped[str] = mapped_column("AnalysisType", String(80), nullable=False)
    summary: Mapped[str] = mapped_column("Summary", String(1000), nullable=False)
    details: Mapped[dict[str, Any]] = mapped_column(
        "Details", JSON().with_variant(JSONB, "postgresql"), nullable=False
    )
    created_at: Mapped[datetime] = mapped_column(
        "CreatedAt", DateTime(timezone=True), nullable=False, server_default=func.now()
    )
