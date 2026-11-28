-- Migration: Add ImageUrl/AvatarUrl fields to game entities
-- Generated: 2026-11-26

-- 1. Add ImageUrl to Upgrades table
ALTER TABLE "Upgrades" ADD COLUMN IF NOT EXISTS "ImageUrl" TEXT;

-- 2. Add ImageUrl to GameEvents table
ALTER TABLE "GameEvents" ADD COLUMN IF NOT EXISTS "ImageUrl" TEXT;

-- 3. Add ImageUrl to Achievements table (entity: GameAchievement)
ALTER TABLE "Achievements" ADD COLUMN IF NOT EXISTS "ImageUrl" TEXT;

-- 4. Add ImageUrl to PlayerCosmetics table
ALTER TABLE "PlayerCosmetics" ADD COLUMN IF NOT EXISTS "ImageUrl" TEXT;

-- 5. Add ImageUrl to Missions table
ALTER TABLE "Missions" ADD COLUMN IF NOT EXISTS "ImageUrl" TEXT;

-- 5b. Add ImageUrl to PlayerIngredients table
ALTER TABLE "PlayerIngredients" ADD COLUMN IF NOT EXISTS "ImageUrl" TEXT;

-- 6. Add AvatarUrl to Users table (for player profile)
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "AvatarUrl" TEXT;

-- 7. Create indexes for performance (optional)
CREATE INDEX IF NOT EXISTS "IX_Upgrades_ImageUrl" ON "Upgrades" ("ImageUrl") WHERE "ImageUrl" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_GameEvents_ImageUrl" ON "GameEvents" ("ImageUrl") WHERE "ImageUrl" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_Achievements_ImageUrl" ON "Achievements" ("ImageUrl") WHERE "ImageUrl" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_PlayerCosmetics_ImageUrl" ON "PlayerCosmetics" ("ImageUrl") WHERE "ImageUrl" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_Missions_ImageUrl" ON "Missions" ("ImageUrl") WHERE "ImageUrl" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_Users_AvatarUrl" ON "Users" ("AvatarUrl") WHERE "AvatarUrl" IS NOT NULL;

-- 8. Update existing records with default values (optional - can be done via application logic)
-- UPDATE "Upgrades" SET "ImageUrl" = '/assets/images/upgrades/default.png' WHERE "ImageUrl" IS NULL;
-- UPDATE "GameEvents" SET "ImageUrl" = '/assets/images/events/default.png' WHERE "ImageUrl" IS NULL;
-- UPDATE "Achievements" SET "ImageUrl" = '/assets/images/achievements/default.png' WHERE "ImageUrl" IS NULL;
-- UPDATE "PlayerCosmetics" SET "ImageUrl" = '/assets/images/cosmetics/default.png' WHERE "ImageUrl" IS NOT NULL;
-- UPDATE "Missions" SET "ImageUrl" = '/assets/images/missions/default.png' WHERE "ImageUrl" IS NULL;
-- UPDATE "PlayerIngredients" SET "ImageUrl" = '/assets/images/ingredients/default.png' WHERE "ImageUrl" IS NULL;
-- UPDATE "Users" SET "AvatarUrl" = '/assets/images/avatars/default.png' WHERE "AvatarUrl" IS NULL;

-- Rollback (if needed)
-- ALTER TABLE "Upgrades" DROP COLUMN IF EXISTS "ImageUrl";
-- ALTER TABLE "GameEvents" DROP COLUMN IF EXISTS "ImageUrl";
-- ALTER TABLE "Achievements" DROP COLUMN IF EXISTS "ImageUrl";
-- ALTER TABLE "PlayerCosmetics" DROP COLUMN IF EXISTS "ImageUrl";
-- ALTER TABLE "Missions" DROP COLUMN IF EXISTS "ImageUrl";
-- ALTER TABLE "PlayerIngredients" DROP COLUMN IF EXISTS "ImageUrl";
-- ALTER TABLE "Users" DROP COLUMN IF EXISTS "AvatarUrl";