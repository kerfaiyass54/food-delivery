# Restaurant Analysis API

A FastAPI service that reads restaurants from the existing FoodDelivery PostgreSQL database, creates a transparent profile analysis from fields on `FoodDelivery.Api.Entities.Restaurant`, saves each result, and serves analysis history and details.

The source entity has no orders, revenue, reviews, or customer metrics. This API therefore reports profile and operational configuration findings only; it does not present unsupported business performance claims.

## Requirements and setup

- Python 3.11+
- Access to the PostgreSQL database used by `FoodDelivery.Api`

From this directory, create a virtual environment, install the package, then configure the database URL:

```powershell
py -3.11 -m venv .venv
.venv\Scripts\Activate.ps1
python -m pip install --upgrade pip
python -m pip install -e .
Copy-Item .env.example .env
```

Set `DATABASE_URL` in `.env` to the same database used by the .NET API. Keep real credentials out of source control. The application reads environment variables and `.env`; settings are defined in `app/core/config.py`.

Apply the analysis-table migration, then start the API:

```powershell
alembic upgrade head
uvicorn app.main:app --reload
```

The service listens on `http://127.0.0.1:8000`; interactive API docs are available at `/docs`.

## API

All routes are under `/api/v1`.

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/restaurants/{restaurant_id}/analyses` | Generate and persist a fresh analysis for an existing restaurant. |
| `GET` | `/restaurants/{restaurant_id}/analyses` | List all analyses, newest first, including the restaurant name and creation date. Optional `limit` (1–100) and `offset` support pagination. |
| `GET` | `/restaurants/{restaurant_id}/analyses/{analysis_id}` | Retrieve the complete saved analysis. |

Missing restaurants and analyses return `404`. A generated analysis includes a snapshot of the restaurant fields used, a summary, profile metrics, findings, and recommendations. Analysis types are versioned in the response so the stored record remains interpretable if the rules evolve.

## Configuration

`DATABASE_URL` is required. `APP_NAME`, `API_V1_PREFIX`, and `LOG_LEVEL` are optional. The database session is request-scoped and closed after each request. The migration only creates the separate `RestaurantAnalyses` table; it does not create or alter the .NET application's tables.

## Production notes

Run Alembic migrations as a deployment step. Run Uvicorn behind a production ASGI process manager/reverse proxy, use TLS and secret management, and add the same identity/authorization policy used by the owner-facing application before exposing owner data. The .NET API currently has no shared auth integration in this service, so access control must be supplied at the deployment boundary before public exposure.
