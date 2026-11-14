from typing import Annotated

from fastapi import APIRouter, Depends, Query, status

from app.api.dependencies import get_analysis_service
from app.schemas.analysis import AnalysisResponse, AnalysisSummary
from app.services.analysis_service import AnalysisService

router = APIRouter(prefix="/restaurants/{restaurant_id}/analyses", tags=["restaurant analyses"])


@router.post("", response_model=AnalysisResponse, status_code=status.HTTP_201_CREATED)
def create_analysis(
    restaurant_id: int,
    service: Annotated[AnalysisService, Depends(get_analysis_service)],
) -> AnalysisResponse:
    return service.create(restaurant_id)


@router.get("", response_model=list[AnalysisSummary])
def list_analyses(
    restaurant_id: int,
    service: Annotated[AnalysisService, Depends(get_analysis_service)],
    limit: Annotated[int | None, Query(ge=1, le=100)] = None,
    offset: Annotated[int, Query(ge=0)] = 0,
) -> list[AnalysisSummary]:
    return service.list_for_restaurant(restaurant_id, limit=limit, offset=offset)


@router.get("/{analysis_id}", response_model=AnalysisResponse)
def get_analysis(
    restaurant_id: int,
    analysis_id: int,
    service: Annotated[AnalysisService, Depends(get_analysis_service)],
) -> AnalysisResponse:
    return service.get(restaurant_id, analysis_id)
