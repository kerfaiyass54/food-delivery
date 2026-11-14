-- Two non-overlapping off days per seeded restaurant.
INSERT INTO "RestaurantOffDays" ("RestaurantId", "DayId")
SELECT r, ((r + offset - 1) % 7) + 1
FROM generate_series(1, 250) AS r
CROSS JOIN generate_series(5, 6) AS offset
ON CONFLICT DO NOTHING;
