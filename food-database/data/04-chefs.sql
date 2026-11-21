-- 300 chefs, including independent applicants (RestaurantId NULL) and restaurant staff.
INSERT INTO "Chefs" ("Id", "Name", "Speciality", "Experience", "Salary", "RestaurantId")
SELECT n,
       'Chef ' || lpad(n::text, 3, '0'),
       (ARRAY['Grill', 'Pastry', 'Tunisian cuisine', 'Italian cuisine', 'Seafood'])[((n - 1) % 5) + 1],
       n % 41,
       (700 + n * 9.75)::numeric(18,2),
       CASE WHEN n % 10 = 0 THEN NULL ELSE ((n - 1) % 250) + 1 END
FROM generate_series(1, 300) AS n
ON CONFLICT DO NOTHING;
