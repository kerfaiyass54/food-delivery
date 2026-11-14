from fastapi import Depends
from sqlalchemy.orm import Session

from app.db.session import get_db
from app.repositories.analysis_repository import AnalysisRepository
from app.services.analysis_service import AnalysisService


def get_analysis_service(db: Session = Depends(get_db)) -> AnalysisService:
    return AnalysisService(AnalysisRepository(db))
