-- Explicit deterministic seed IDs must not leave identity sequences behind the inserted rows.
SELECT setval(pg_get_serial_sequence('"Owners"', 'Id'), (SELECT max("Id") FROM "Owners"), true);
SELECT setval(pg_get_serial_sequence('"Days"', 'Id'), (SELECT max("Id") FROM "Days"), true);
SELECT setval(pg_get_serial_sequence('"Restaurants"', 'Id'), (SELECT max("Id") FROM "Restaurants"), true);
SELECT setval(pg_get_serial_sequence('"Chefs"', 'Id'), (SELECT max("Id") FROM "Chefs"), true);
SELECT setval(pg_get_serial_sequence('"Candidatures"', 'Id'), (SELECT max("Id") FROM "Candidatures"), true);
SELECT setval(pg_get_serial_sequence('"Clients"', 'Id'), (SELECT max("Id") FROM "Clients"), true);
SELECT setval(pg_get_serial_sequence('"Products"', 'Id'), (SELECT max("Id") FROM "Products"), true);
SELECT setval(pg_get_serial_sequence('"Orders"', 'Id'), (SELECT max("Id") FROM "Orders"), true);
SELECT setval(pg_get_serial_sequence('"Purchases"', 'Id'), (SELECT max("Id") FROM "Purchases"), true);
