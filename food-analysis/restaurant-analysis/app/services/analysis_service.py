from datetime import UTC, datetime
from decimal import Decimal

from fastapi import HTTPException, status

from app.models.analysis import RestaurantAnalysis
from app.models.restaurant import RestaurantRecord
from app.repositories.analysis_repository import AnalysisRepository


class AnalysisService:
    """Builds and persists explainable analyses from the Restaurant entity fields."""

    ANALYSIS_TYPE = "restaurant_profile_v1"
    STATUS_NAMES = {0: "closed", 1: "repair", 2: "open"}

    def __init__(self, repository: AnalysisRepository) -> None:
        self._repository = repository

    def create(self, restaurant_id: int) -> RestaurantAnalysis:
        restaurant = self._require_restaurant(restaurant_id)
        details, summary = self._analyze(restaurant)
        analysis = RestaurantAnalysis(
            restaurant_id=restaurant.id,
            restaurant_name=restaurant.name,
            analysis_type=self.ANALYSIS_TYPE,
            summary=summary,
            details=details,
        )
        return self._repository.add(analysis)

    def list_for_restaurant(
        self, restaurant_id: int, *, limit: int | None, offset: int
    ) -> list[RestaurantAnalysis]:
        self._require_restaurant(restaurant_id)
        return self._repository.list_for_restaurant(restaurant_id, limit=limit, offset=offset)

    def get(self, restaurant_id: int, analysis_id: int) -> RestaurantAnalysis:
        self._require_restaurant(restaurant_id)
        analysis = self._repository.get_for_restaurant(restaurant_id, analysis_id)
        if analysis is None:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Analysis not found")
        return analysis

    def _require_restaurant(self, restaurant_id: int) -> RestaurantRecord:
        restaurant = self._repository.get_restaurant(restaurant_id)
        if restaurant is None:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Restaurant not found")
        return restaurant

    @classmethod
    def _analyze(cls, restaurant: RestaurantRecord) -> tuple[dict, str]:
        status_name = cls.STATUS_NAMES.get(restaurant.status, "unknown")
        opening_hours = None
        findings: list[dict[str, str]] = []
        recommendations: list[str] = []

        if restaurant.time_open and restaurant.time_close:
            opening_hours = {
                "opens": restaurant.time_open.isoformat(timespec="minutes"),
                "closes": restaurant.time_close.isoformat(timespec="minutes"),
            }
            if restaurant.time_close <= restaurant.time_open:
                findings.append(
                    {
                        "category": "attention",
                        "title": "Opening hours cross midnight or need review",
                        "description": "The closing time is not later than the opening time. Confirm that this represents the intended overnight schedule.",
                    }
                )
            else:
                findings.append(
                    {
                        "category": "information",
                        "title": "Opening hours are configured",
                        "description": f"The stored daily schedule runs from {opening_hours['opens']} to {opening_hours['closes']}; operating days are not included in this analysis.",
                    }
                )
        elif restaurant.time_open or restaurant.time_close:
            findings.append(
                {
                    "category": "attention",
                    "title": "Opening hours are incomplete",
                    "description": "Only one of the opening and closing times is set.",
                }
            )
            recommendations.append("Set both opening and closing times, or clear both if hours are not yet decided.")
        else:
            findings.append(
                {
                    "category": "attention",
                    "title": "Opening hours are not configured",
                    "description": "No opening or closing time is stored for this restaurant.",
                }
            )
            recommendations.append("Add opening and closing times so customers and staff can understand the schedule.")

        if status_name == "open":
            findings.append(
                {
                    "category": "strength",
                    "title": "Restaurant is marked open",
                    "description": "The stored restaurant status is Open.",
                }
            )
        elif status_name == "repair":
            findings.append(
                {
                    "category": "attention",
                    "title": "Restaurant is marked under repair",
                    "description": "The stored status is Repair; confirm it is updated when work is complete.",
                }
            )
            recommendations.append("Review the status after repairs so it reflects the current operating state.")
        elif status_name == "closed":
            findings.append(
                {
                    "category": "information",
                    "title": "Restaurant is marked closed",
                    "description": "The stored restaurant status is Closed.",
                }
            )
        else:
            findings.append(
                {
                    "category": "attention",
                    "title": "Restaurant status is unrecognized",
                    "description": f"The database contains status value {restaurant.status}.",
                }
            )

        budget_per_seat = restaurant.budget / restaurant.capacity if restaurant.capacity > 0 else None
        if restaurant.capacity <= 10:
            capacity_profile = "small"
            recommendations.append("Check that the recorded capacity matches the current seating arrangement.")
        elif restaurant.capacity <= 50:
            capacity_profile = "medium"
        else:
            capacity_profile = "large"

        findings.append(
            {
                "category": "information",
                "title": f"{capacity_profile.capitalize()} recorded capacity",
                "description": f"The restaurant is configured for {restaurant.capacity} seats. Capacity bands are descriptive rules of thumb, not performance benchmarks.",
            }
        )

        if not restaurant.address.strip():
            findings.append(
                {
                    "category": "attention",
                    "title": "Address is empty",
                    "description": "No usable address is stored on the restaurant record.",
                }
            )
            recommendations.append("Add a complete address to the restaurant profile.")
        if not restaurant.nationality or not restaurant.nationality.strip():
            findings.append(
                {
                    "category": "information",
                    "title": "Cuisine or nationality is unspecified",
                    "description": "The optional Nationality field is empty.",
                }
            )
        if restaurant.budget == Decimal("0"):
            findings.append(
                {
                    "category": "attention",
                    "title": "Budget is zero",
                    "description": "The restaurant budget is stored as zero; confirm whether that is intentional.",
                }
            )
            recommendations.append("Confirm the restaurant budget is recorded in the expected currency and units.")

        if not recommendations:
            recommendations.append("Keep the restaurant profile current as its capacity, status, budget, and hours change.")

        snapshot = {
            "id": restaurant.id,
            "name": restaurant.name,
            "address": restaurant.address,
            "capacity": restaurant.capacity,
            "date_of_add": restaurant.date_of_add.isoformat(),
            "status": {"code": restaurant.status, "name": status_name},
            "opening_hours": opening_hours,
            "nationality": restaurant.nationality,
            "budget": str(restaurant.budget),
            "owner_id": restaurant.owner_id,
        }
        summary = (
            f"{restaurant.name}: {status_name} profile with {restaurant.capacity} seats; "
            f"{len([item for item in findings if item['category'] == 'attention'])} item(s) to review."
        )
        details = {
            "schema_version": "1.0",
            "generated_at": datetime.now(UTC).isoformat(),
            "restaurant_snapshot": snapshot,
            "metrics": {
                "days_since_added": max((datetime.now(UTC) - restaurant.date_of_add).days, 0),
                "capacity_profile": capacity_profile,
                "budget_per_seat": str(budget_per_seat) if budget_per_seat is not None else None,
                "has_complete_opening_hours": bool(restaurant.time_open and restaurant.time_close),
                "has_nationality": bool(restaurant.nationality and restaurant.nationality.strip()),
            },
            "findings": findings,
            "recommendations": recommendations,
        }
        return details, summary
