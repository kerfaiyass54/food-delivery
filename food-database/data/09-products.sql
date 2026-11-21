-- One stocked product per chef. Stock represents the available balance after sample history.
INSERT INTO "Products" ("Id", "Name", "Type", "Stock", "Price", "CookDate", "ChefId")
SELECT n,
       (ARRAY['Saffron couscous', 'Harissa grilled chicken', 'Lemon olive cake', 'Herb brik', 'Market garden bowl'])[((n - 1) % 5) + 1]
           || ' ' || lpad(n::text, 3, '0'),
       (ARRAY['Main course', 'Grill', 'Dessert', 'Street food', 'Vegetarian'])[((n - 1) % 5) + 1],
       25 + (n % 17),
       (ARRAY[8.50, 12.00, 5.75, 4.25, 9.90])[((n - 1) % 5) + 1],
       now() - make_interval(hours => n % 72),
       n
FROM generate_series(1, 300) AS n
ON CONFLICT DO NOTHING;
