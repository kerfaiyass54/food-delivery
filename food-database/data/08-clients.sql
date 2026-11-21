-- 250 client profiles using ClientStatus enum names stored by EF's string conversion.
INSERT INTO "Clients" ("Id", "Name", "City", "Nationality", "Budget", "Weight", "Status")
SELECT n,
       'Client ' || lpad(n::text, 3, '0'),
       (ARRAY['Tunis', 'Sfax', 'Sousse', 'Bizerte', 'Gabes'])[((n - 1) % 5) + 1],
       (ARRAY['Tunisian', 'Italian', 'French', 'Algerian', 'Moroccan'])[((n - 1) % 5) + 1],
       (5000 + n * 35)::numeric(18,2),
       (45 + (n % 65) + ((n % 4) * 0.25))::numeric(8,2),
       (ARRAY['Sick', 'Normal', 'Diet'])[(n % 3) + 1]
FROM generate_series(1, 250) AS n
ON CONFLICT DO NOTHING;
