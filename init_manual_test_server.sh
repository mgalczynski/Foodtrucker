#!/bin/sh

curl -d '{"firstName":"Mirosław", "lastName":"Gałczyński", "email":"contact@miroslawgalczynski.com", "password":"P@ssw0rd1"}' -H "Content-Type: application/json" -X POST http://localhost:8080/api/auth/registerStaff
curl -d '{"firstName":"Second", "lastName":"User", "email":"2.user@miroslawgalczynski.com", "password":"P@ssw0rd1"}' -H "Content-Type: application/json" -X POST http://localhost:8080/api/auth/registerStaff
curl -d '{"firstName":"Third", "lastName":"User", "email":"3.user@miroslawgalczynski.com", "password":"P@ssw0rd1"}' -H "Content-Type: application/json" -X POST http://localhost:8080/api/auth/registerStaff
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U Foodtrucker < Dumps/Foodtrucks.d.sql
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U Foodtrucker < Dumps/PresencesOrUnavailabilities.d.sql
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U Foodtrucker < Scripts/PresencesOrUnavailabilities.sql
docker exec -i foodtrucker_db_1 psql -d Foodtrucker -U Foodtrucker < Scripts/Ownerships.sql
