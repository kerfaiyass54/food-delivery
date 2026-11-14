-- One product line per order. Each product belongs to the chef assigned to that order.
INSERT INTO "OrderProducts" ("OrderId", "ProductId", "Quantity")
SELECT n, n, 1 + (n % 2)
FROM generate_series(1, 300) AS n
ON CONFLICT DO NOTHING;
