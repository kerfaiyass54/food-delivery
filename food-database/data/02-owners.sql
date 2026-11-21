-- 250 deterministic owner records; Email uniqueness is enforced by the index script.
INSERT INTO "Owners" ("Id", "Name", "Age", "Phone", "Email")
SELECT n,
       'Owner ' || lpad(n::text, 3, '0'),
       18 + (n % 60),
       '+216 20 ' || lpad(((n * 37) % 1000000)::text, 6, '0'),
       'owner' || lpad(n::text, 3, '0') || '@example.test'
FROM generate_series(1, 250) AS n
ON CONFLICT DO NOTHING;
