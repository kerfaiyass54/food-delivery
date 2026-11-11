# Local infrastructure

This Compose stack is for local development. It runs PostgreSQL, pgAdmin, Kafka, Redis, Prometheus, Grafana, and Keycloak. It does not run the FoodDelivery API.

## Start

From the repository root, copy `.env.example` to `.env`, replace every `replace-with-...` value with a long random password, and run:

```powershell
docker compose up -d
docker compose ps
```

The `.env` file is ignored by Git. Stop the services with `docker compose down`; named volumes keep their data. `docker compose down -v` deletes all database and service data.

## Local endpoints

| Service | URL / address | Notes |
| --- | --- | --- |
| PostgreSQL | `localhost:5432` | App database `fooddelivery`; user `fooddelivery`. Keycloak uses its own `keycloak` database and user. |
| pgAdmin | <http://localhost:5050> | Sign in with `PGADMIN_EMAIL` and `PGADMIN_PASSWORD`. Add server host `postgres`, port `5432`, database `fooddelivery`, username `fooddelivery`. |
| Kafka | `localhost:9094` | Host clients use this address; other Compose services use `kafka:9092`. |
| Redis | `localhost:6379` | Authentication uses `REDIS_PASSWORD`. |
| Prometheus | <http://localhost:9090> | Scrapes itself and Keycloak. |
| Grafana | <http://localhost:3000> | Sign in with `GRAFANA_ADMIN_USER` and `GRAFANA_ADMIN_PASSWORD`; Prometheus is provisioned as the default data source. |
| Keycloak | <http://localhost:8080> | Sign in with `KEYCLOAK_ADMIN` and `KEYCLOAK_ADMIN_PASSWORD`. |

Container-to-container connections use service names, never `localhost`: PostgreSQL `postgres:5432`, Redis `redis:6379`, Kafka `kafka:9092`, and Keycloak `keycloak:8080`.

The database initialization script creates app and Keycloak databases on the first start only. If you change database passwords after the named PostgreSQL volume has been initialized, update the database roles manually or recreate the volume. The Keycloak service runs in development mode, and Kafka uses unauthenticated plaintext listeners; keep these ports local and do not use this configuration for production.

## Run the API against PostgreSQL

The API reads `ConnectionStrings:FoodDelivery` from .NET user secrets in Development. To store the app database connection using the password already in `.env`, run in PowerShell from the repository root:

```powershell
dotnet user-secrets init
$dbPassword = (Get-Content .env | Where-Object { $_ -match '^FOODDELIVERY_DB_PASSWORD=' }) -replace '^FOODDELIVERY_DB_PASSWORD=', ''
dotnet user-secrets set 'ConnectionStrings:FoodDelivery' "Host=localhost;Port=5432;Database=fooddelivery;Username=fooddelivery;Password=$dbPassword"
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet ef database update
dotnet run
```

`user-secrets init` is only needed once per checkout. The schema migration seeds the weekdays (IDs 1–7). Restaurant endpoints include `GET /api/restaurants/{id}`, `GET /api/owners/{ownerId}/restaurant`, `PUT /api/restaurants/{id}/capacity`, `PUT /api/restaurants/{id}/days`, and `PUT /api/restaurants/{id}/opening-hours`. Chef endpoints are `GET /api/restaurants/{id}/chefs`, `GET /api/restaurants/{id}/chefs/{chefId}`, and `POST /api/restaurants/{id}/chefs/{chefId}`. Candidature endpoints are `POST /api/restaurants/{id}/candidatures`, `GET /api/restaurants/{id}/candidatures`, `GET /api/restaurants/{id}/candidatures/{candidatureId}`, `GET /api/restaurants/{id}/candidatures/{candidatureId}/chef`, and `PUT /api/restaurants/{id}/candidatures/{candidatureId}/refuse`. A new candidature starts as pending. For request bodies and behavior, see `DOCKER-COMPOSE-STEPS.txt`.
