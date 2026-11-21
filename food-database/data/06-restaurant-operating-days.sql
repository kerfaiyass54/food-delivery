-- Five operating days per seeded restaurant; canonical Day IDs 1 through 7.
INSERT INTO "RestaurantOperatingDays" ("RestaurantId", "DayId")
SELECT r, ((r + offset - 1) % 7) + 1
FROM generate_series(1, 250) AS r
CROSS JOIN generate_series(0, 4) AS offset
ON CONFLICT DO NOTHING;
