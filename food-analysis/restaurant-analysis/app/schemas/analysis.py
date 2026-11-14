from datetime import datetime
from typing import Any, Literal

from pydantic import BaseModel, ConfigDict


class Finding(BaseModel):
    category: Literal["strength", "attention", "information"]
    title: str
    description: str


class AnalysisDetails(BaseModel):
    model_config = ConfigDict(extra="forbid")

    schema_version: str
    generated_at: datetime
    restaurant_snapshot: dict[str, Any]
    metrics: dict[str, Any]
    findings: list[Finding]
    recommendations: list[str]


class AnalysisSummary(BaseModel):
    id: int
    restaurant_id: int
    restaurant_name: str
    analysis_type: str
    summary: str
    created_at: datetime

    model_config = ConfigDict(from_attributes=True)


class AnalysisResponse(AnalysisSummary):
    details: AnalysisDetails
