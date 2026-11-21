# FoodDelivery PostgreSQL database

This directory contains SQL for the schema, sample data, indexes, and validation of the entities in `FoodDelivery.Api/Entities`. Identifiers, types, constraints, and delete behavior match `FoodDelivery.Api/Data/FoodDeliveryDbContext.cs` and all EF Core migrations, including the nullable product-price migration.

## Layout

- `init/`: creates the application tables and verifies seed counts and day relationships.
- `data/`: deterministic, repeatable seed statements, one file per entity/table.
- `indexes/`: indexes from the EF model plus indexes for service query paths.
- `scripts/initialize.sh`: applies the schema, seed files in order, indexes, and validation.

## Seed coverage

The sample data contains 250 owners, 250 restaurants, 300 chefs, 500 candidatures, 1,250 restaurant operating-day links, 500 restaurant off-day links, 250 clients, 300 products, 300 orders, 500 purchases, and 300 order-product links. Every entity table has at least 250 rows except `Days`, which has exactly seven canonical weekdays; its unique name index and lookup semantics make additional rows invalid. Commerce order lines point to products managed by the same chef as the order.

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

The schema and seed inserts use `IF NOT EXISTS` / `ON CONFLICT DO NOTHING` behavior, so rerunning the initializer preserves existing records. Product prices are stored on `Products.Price`; existing records may keep `NULL` until a chef sets a price, while seeded sample products receive deterministic prices. It uses the `fooddelivery` database owner created by the existing bootstrap script. The SQL matches the initial and commerce EF migrations and records `20261110205112_InitialCreate`, `20261114143456_AddClientsProductsPurchasesOrders`, and `20261121180000_AddProductPrice` in `__EFMigrationsHistory`, so later `dotnet ef database update` calls do not attempt to recreate these tables.

## Indexes and service query paths

The index script retains the model's owner, restaurant, chef, candidature, product, purchase, order, and join-table indexes. It adds composite date indexes for client and chef order feeds and client purchase history, plus a partial name index for the available-product menu query. Restaurant sort columns receive B-tree indexes, and trigram GIN indexes support substring restaurant search.

Use `init/02-validate-seed-data.sql` to confirm minimum row counts, seven weekday rows, valid order-to-chef/product relationships, non-negative stock and budgets, and that no seeded restaurant day is both an operating day and an off day.
