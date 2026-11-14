-- One restaurant for each seeded owner. Status codes match RestaurantStatus (Closed=0, Repair=1, Open=2).
INSERT INTO "Restaurants" (
    "Id", "Name", "Address", "Capacity", "DateOfAdd", "Status", "TimeOpen", "TimeClose",
    "Nationality", "Budget", "OwnerId"
)
SELECT n,
       'Restaurant ' || lpad(n::text, 3, '0'),
       'No. ' || n || ', Avenue Habib Bourguiba, Tunis',
       10 + (n % 91),
       now() - make_interval(days => n),
       n % 3,
       time '08:00' + ((n % 3) * interval '1 hour'),
       time '20:00' + ((n % 3) * interval '1 hour'),
       (ARRAY['Tunisian', 'Italian', 'French', 'Turkish', 'Japanese'])[((n - 1) % 5) + 1],
       (5000 + n * 125.50)::numeric(18,2),
       n
FROM generate_series(1, 250) AS n
ON CONFLICT DO NOTHING;
