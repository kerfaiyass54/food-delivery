DO $$
DECLARE
    table_name text;
    row_count bigint;
BEGIN
    IF (SELECT count(*) FROM "Days") <> 7 THEN
        RAISE EXCEPTION 'Expected exactly seven canonical rows in Days';
    END IF;

    FOREACH table_name IN ARRAY ARRAY[
        'Owners', 'Restaurants', 'Chefs', 'Candidatures',
        'RestaurantOperatingDays', 'RestaurantOffDays', 'Clients',
        'Products', 'Orders', 'Purchases', 'OrderProducts'
    ] LOOP
        EXECUTE format('SELECT count(*) FROM %I', table_name) INTO row_count;
        IF row_count < 250 THEN
            RAISE EXCEPTION 'Expected at least 250 rows in %, found %', table_name, row_count;
        END IF;
    END LOOP;

    IF EXISTS (
        SELECT 1
        FROM "RestaurantOperatingDays" operating
        JOIN "RestaurantOffDays" off_day
          ON off_day."RestaurantId" = operating."RestaurantId"
         AND off_day."DayId" = operating."DayId"
    ) THEN
        RAISE EXCEPTION 'A restaurant day cannot be both an operating day and an off day';
    END IF;
    IF EXISTS (
        SELECT 1
        FROM "OrderProducts" line
        JOIN "Orders" order_row ON order_row."Id" = line."OrderId"
        JOIN "Products" product ON product."Id" = line."ProductId"
        WHERE order_row."ChefId" <> product."ChefId"
    ) THEN
        RAISE EXCEPTION 'Each order line product must belong to the order chef';
    END IF;

    IF EXISTS (SELECT 1 FROM "Products" WHERE "Stock" < 0)
       OR EXISTS (SELECT 1 FROM "Clients" WHERE "Budget" < 0) THEN
        RAISE EXCEPTION 'Seeded inventory and client budgets must be non-negative';
    END IF;
END $$;
