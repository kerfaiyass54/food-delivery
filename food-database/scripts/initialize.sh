#!/bin/sh
set -eu

: "${FOODDELIVERY_DB_USER:=fooddelivery}"
: "${FOODDELIVERY_DB_NAME:=fooddelivery}"

database_dir="$(CDPATH= cd -- "$(dirname -- "$0")/.." && pwd)"

# The schema is installed before seed data; the indexes are added after bulk inserts.
psql --set=ON_ERROR_STOP=1 --username="$FOODDELIVERY_DB_USER" --dbname="$FOODDELIVERY_DB_NAME" \
    --file="$database_dir/init/01-create-tables.sql"

for seed_file in "$database_dir"/data/*.sql; do
    psql --set=ON_ERROR_STOP=1 --username="$FOODDELIVERY_DB_USER" --dbname="$FOODDELIVERY_DB_NAME" \
        --file="$seed_file"
done

psql --set=ON_ERROR_STOP=1 --username="$FOODDELIVERY_DB_USER" --dbname="$FOODDELIVERY_DB_NAME" \
    --file="$database_dir/indexes/01-service-query-indexes.sql"
psql --set=ON_ERROR_STOP=1 --username="$FOODDELIVERY_DB_USER" --dbname="$FOODDELIVERY_DB_NAME" \
    --file="$database_dir/init/02-validate-seed-data.sql"
