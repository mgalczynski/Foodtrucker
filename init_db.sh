#!/bin/sh

docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U postgres < Dumps/Foodtrucks.d.sql
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U postgres < Dumps/PresencesOrUnavailabilities.d.sql
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U postgres < Scripts/PresencesOrUnavailabilities.sql
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U postgres < Scripts/Ownerships.sql
