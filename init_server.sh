#!/bin/sh

curl -kd '{"firstName":"Mirosław", "lastName":"Gałczyński", "email":"contact@miroslawgalczynski.com", "password":"P@ssw0rd1"}' -H "Content-Type: application/json" -X POST https://foodtrucker.miroslawgalczynski.com/api/auth/registerStaff
ssh -p 45 -t foodtrucker.miroslawgalczynski.com docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U Foodtrucker < Dumps/Foodtrucks.d.sql
ssh -p 45 -t foodtrucker.miroslawgalczynski.com docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U Foodtrucker < Dumps/PresencesOrUnavailabilities.d.sql
ssh -p 45 -t foodtrucker.miroslawgalczynski.com docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U Foodtrucker < Scripts/PresencesOrUnavailabilities.sql
ssh -p 45 -t foodtrucker.miroslawgalczynski.com docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U Foodtrucker < Scripts/Ownerships.sql
