-- Indexes already defined by FoodDeliveryDbContext / its initial EF migration.
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Owners_Email" ON "Owners" ("Email");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Restaurants_OwnerId" ON "Restaurants" ("OwnerId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Days_Name" ON "Days" ("Name");
CREATE INDEX IF NOT EXISTS "IX_Chefs_RestaurantId" ON "Chefs" ("RestaurantId");
CREATE INDEX IF NOT EXISTS "IX_Candidatures_ChefId_RestaurantId" ON "Candidatures" ("ChefId", "RestaurantId");
CREATE INDEX IF NOT EXISTS "IX_Candidatures_RestaurantId" ON "Candidatures" ("RestaurantId");
CREATE INDEX IF NOT EXISTS "IX_RestaurantOperatingDays_DayId" ON "RestaurantOperatingDays" ("DayId");
CREATE INDEX IF NOT EXISTS "IX_RestaurantOffDays_DayId" ON "RestaurantOffDays" ("DayId");

-- Service query paths: chef lists are scoped to a restaurant and ordered by name.
CREATE INDEX IF NOT EXISTS "IX_Chefs_RestaurantId_Name" ON "Chefs" ("RestaurantId", "Name");

-- Candidature lists are filtered by restaurant or chef and sorted newest first.
CREATE INDEX IF NOT EXISTS "IX_Candidatures_RestaurantId_Date" ON "Candidatures" ("RestaurantId", "Date" DESC, "Id" DESC);
CREATE INDEX IF NOT EXISTS "IX_Candidatures_ChefId_Date" ON "Candidatures" ("ChefId", "Date" DESC, "Id" DESC);

-- SearchAsync supports substring filtering and sorting on these columns.
CREATE INDEX IF NOT EXISTS "IX_Restaurants_Name" ON "Restaurants" ("Name");
CREATE INDEX IF NOT EXISTS "IX_Restaurants_Address" ON "Restaurants" ("Address");
CREATE INDEX IF NOT EXISTS "IX_Restaurants_Nationality" ON "Restaurants" ("Nationality");
CREATE INDEX IF NOT EXISTS "IX_Restaurants_Budget" ON "Restaurants" ("Budget");
CREATE INDEX IF NOT EXISTS "IX_Restaurants_DateOfAdd" ON "Restaurants" ("DateOfAdd");

-- Contains() is translated to LIKE '%term%'; trigram GIN indexes support those filters.
CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE INDEX IF NOT EXISTS "IX_Restaurants_Name_Trgm" ON "Restaurants" USING gin ("Name" gin_trgm_ops);
CREATE INDEX IF NOT EXISTS "IX_Restaurants_Address_Trgm" ON "Restaurants" USING gin ("Address" gin_trgm_ops);
CREATE INDEX IF NOT EXISTS "IX_Restaurants_Nationality_Trgm" ON "Restaurants" USING gin ("Nationality" gin_trgm_ops);
