# FoodDelivery PostgreSQL database

This directory contains SQL for the schema, sample data, indexes, and validation of the entities in `FoodDelivery.Api/Entities`. Identifiers, types, constraints, and delete behavior match `FoodDelivery.Api/Data/FoodDeliveryDbContext.cs` and its initial EF Core migration.

## Layout

- `init/`: creates the application tables and verifies seed counts and day relationships.
- `data/`: deterministic, repeatable seed statements, one file per entity/table.
- `indexes/`: indexes from the EF model plus indexes for service query paths.
- `scripts/initialize.sh`: applies the schema, seed files in order, indexes, and validation.

## Seed coverage

The sample data contains 250 owners, 250 restaurants, 300 chefs, 500 candidatures, 1,250 restaurant operating-day links, and 500 restaurant off-day links. The `Days` table has exactly seven canonical weekday rows. Its unique name index and the entity's weekday lookup semantics make additional rows invalid.

The sample values are deterministic development data. Owner emails use the reserved `.test` domain. Do not use these records as production customer data.

## Initialize with Docker Compose

The existing PostgreSQL Compose service mounts this directory and runs the initializer after it creates the `fooddelivery` database on the first database-volume initialization:

```powershell
docker compose -f FoodDelivery.Api/docker-compose.yml up -d postgres
```

The PostgreSQL volume is persistent, so its entrypoint does not rerun initialization on an existing volume. Apply the scripts to an existing local volume with:

```powershell
docker compose -f FoodDelivery.Api/docker-compose.yml exec postgres sh /food-database/scripts/initialize.sh
```

The schema and seed inserts use `IF NOT EXISTS` / `ON CONFLICT DO NOTHING` behavior, so rerunning the initializer preserves existing records. It uses the `fooddelivery` database owner created by the existing bootstrap script. The SQL matches the existing initial EF migration and records `20261110205112_InitialCreate` in `__EFMigrationsHistory`, so later `dotnet ef database update` calls do not attempt to recreate these tables.

## Indexes and service query paths

The index script retains the model's unique owner email and restaurant owner indexes, the chef-by-restaurant index, candidature foreign-key indexes, and reverse day-link indexes. It adds composite indexes for chefs by restaurant/name and candidatures by restaurant/date or chef/date, matching their service filters and sort order. Restaurant sort columns receive B-tree indexes. Trigram GIN indexes support `RestaurantRepository.FilterRestaurants`, whose `Contains` filters become substring `LIKE` queries on name, address, and nationality.

Use `init/02-validate-seed-data.sql` to confirm the minimum row counts, seven weekday rows, and that no seeded restaurant day is both an operating day and an off day.
