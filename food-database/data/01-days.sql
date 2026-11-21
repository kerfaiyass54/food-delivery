-- Day is a fixed seven-row lookup entity. Do not fabricate additional weekdays.
INSERT INTO "Days" ("Id", "Name") VALUES
    (1, 'Monday'), (2, 'Tuesday'), (3, 'Wednesday'), (4, 'Thursday'),
    (5, 'Friday'), (6, 'Saturday'), (7, 'Sunday')
ON CONFLICT DO NOTHING;
