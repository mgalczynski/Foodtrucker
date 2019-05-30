INSERT INTO "FoodtruckOwnerships"("FoodtruckId", "UserId", "Type")
SELECT "Foodtrucks"."Id", (SELECT "AspNetUsers"."Id" FROM "AspNetUsers" WHERE "Email" = 'contact@miroslawgalczynski.com'), 'OWNER'
FROM "Foodtrucks";
