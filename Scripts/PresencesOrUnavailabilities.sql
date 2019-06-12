BEGIN TRANSACTION;

UPDATE "PresencesOrUnavailabilities" SET "StartTime" = DATE_TRUNC('day', NOW()), "EndTime" = DATE_TRUNC('day', NOW()) + INTERVAL '1' DAY WHERE "EndTime" IS NOT NULL;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

INSERT INTO "PresencesOrUnavailabilities"("Id", "FoodtruckId", "StartTime", "EndTime", "Title", "Description", "Location")
SELECT UUID_GENERATE_V4(), "FoodtruckId", "EndTime", "EndTime" + INTERVAL '1 day', "Title", "Description", NULL
FROM "PresencesOrUnavailabilities"
WHERE "EndTime" IS NOT NULL;

COMMIT TRANSACTION;