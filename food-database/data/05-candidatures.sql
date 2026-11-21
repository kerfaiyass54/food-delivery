-- 500 applications referencing existing chefs and restaurants; status codes are Pending=0, Accepted=1, Refused=2.
INSERT INTO "Candidatures" ("Id", "Salary", "Date", "Status", "ChefId", "RestaurantId")
SELECT n,
       (900 + n * 7.25)::numeric(18,2),
       now() - make_interval(days => n % 180),
       n % 3,
       ((n - 1) % 300) + 1,
       ((n * 7 - 1) % 250) + 1
FROM generate_series(1, 500) AS n
ON CONFLICT DO NOTHING;
