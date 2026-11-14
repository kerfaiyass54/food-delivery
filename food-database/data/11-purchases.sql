-- 500 purchase records connected to seeded clients and products.
INSERT INTO "Purchases" ("Id", "Date", "Price", "Quantity", "ClientId", "ProductId")
SELECT n,
       now() - make_interval(days => n % 90),
       (8.50 + (n % 42) * 1.25)::numeric(18,2),
       1 + (n % 3),
       ((n - 1) % 250) + 1,
       ((n - 1) % 300) + 1
FROM generate_series(1, 500) AS n
ON CONFLICT DO NOTHING;
