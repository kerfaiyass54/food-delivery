-- Explicit deterministic seed IDs must not leave identity sequences behind the inserted rows.
SELECT setval(pg_get_serial_sequence('"Owners"', 'Id'), (SELECT max("Id") FROM "Owners"), true);
SELECT setval(pg_get_serial_sequence('"Days"', 'Id'), (SELECT max("Id") FROM "Days"), true);
SELECT setval(pg_get_serial_sequence('"Restaurants"', 'Id'), (SELECT max("Id") FROM "Restaurants"), true);
SELECT setval(pg_get_serial_sequence('"Chefs"', 'Id'), (SELECT max("Id") FROM "Chefs"), true);
SELECT setval(pg_get_serial_sequence('"Candidatures"', 'Id'), (SELECT max("Id") FROM "Candidatures"), true);
