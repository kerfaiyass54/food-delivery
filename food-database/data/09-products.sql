-- One stocked product per chef. Stock represents the available balance after sample history.
INSERT INTO "Products" ("Id", "Name", "Type", "Stock", "CookDate", "ChefId")
SELECT n,
       (ARRAY['Saffron couscous', 'Harissa grilled chicken', 'Lemon olive cake', 'Herb brik', 'Market garden bowl'])[((n - 1) % 5) + 1]
           || ' ' || lpad(n::text, 3, '0'),
       (ARRAY['Main course', 'Grill', 'Dessert', 'Street food', 'Vegetarian'])[((n - 1) % 5) + 1],
       25 + (n % 17),
       now() - make_interval(hours => n % 72),
       n
FROM generate_series(1, 300) AS n
ON CONFLICT DO NOTHING;
