from sqlalchemy import select
from sqlalchemy.orm import Session

from app.models.analysis import RestaurantAnalysis
from app.models.restaurant import RestaurantRecord


class AnalysisRepository:
    def __init__(self, session: Session) -> None:
        self._session = session

    def get_restaurant(self, restaurant_id: int) -> RestaurantRecord | None:
        return self._session.get(RestaurantRecord, restaurant_id)

    def add(self, analysis: RestaurantAnalysis) -> RestaurantAnalysis:
        self._session.add(analysis)
        self._session.commit()
        self._session.refresh(analysis)
        return analysis

    def get_for_restaurant(
        self, restaurant_id: int, analysis_id: int
    ) -> RestaurantAnalysis | None:
        statement = select(RestaurantAnalysis).where(
            RestaurantAnalysis.restaurant_id == restaurant_id,
            RestaurantAnalysis.id == analysis_id,
        )
        return self._session.scalar(statement)

    def list_for_restaurant(
        self, restaurant_id: int, *, limit: int | None, offset: int
    ) -> list[RestaurantAnalysis]:
        statement = (
            select(RestaurantAnalysis)
            .where(RestaurantAnalysis.restaurant_id == restaurant_id)
            .order_by(RestaurantAnalysis.created_at.desc(), RestaurantAnalysis.id.desc())
            .offset(offset)
        )
        if limit is not None:
            statement = statement.limit(limit)
        return list(self._session.scalars(statement))
