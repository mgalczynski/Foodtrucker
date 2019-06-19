#!/bin/sh

curl -kd '{"firstName":"Mirosław", "lastName":"Gałczyński", "email":"contact@miroslawgalczynski.com", "password":"P@ssw0rd1"}' -H "Content-Type: application/json" -X POST https://localhost:5001/api/auth/registerStaff
curl -kd '{"firstName":"Second", "lastName":"User", "email":"2.user@miroslawgalczynski.com", "password":"P@ssw0rd1"}' -H "Content-Type: application/json" -X POST https://localhost:5001/api/auth/registerStaff
curl -kd '{"firstName":"Third", "lastName":"User", "email":"3.user@miroslawgalczynski.com", "password":"P@ssw0rd1"}' -H "Content-Type: application/json" -X POST https://localhost:5001/api/auth/registerStaff
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U postgres < Dumps/Foodtrucks.d.sql
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U postgres < Dumps/PresencesOrUnavailabilities.d.sql
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U postgres < Scripts/PresencesOrUnavailabilities.sql
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U postgres < Scripts/Ownerships.sql
