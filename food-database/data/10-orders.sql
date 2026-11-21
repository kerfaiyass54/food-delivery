-- 300 client orders. Chef IDs match the chef who owns each seeded product.
INSERT INTO "Orders" ("Id", "Date", "Time", "Status", "ClientId", "ChefId")
SELECT n,
       now() - make_interval(days => n % 45),
       time '11:00' + ((n % 9) * interval '1 hour'),
       (ARRAY['Pending', 'Launched', 'Cancelled'])[(n % 3) + 1],
       ((n - 1) % 250) + 1,
       n
FROM generate_series(1, 300) AS n
ON CONFLICT DO NOTHING;
