-- Update ImageUrl/AvatarUrl for all tables to point to default logo
-- Run AFTER add_image_url_fields.sql migration

-- Upgrades table
UPDATE "Upgrades" SET "ImageUrl" = '/public/images/somtam-zing-logo.png' WHERE "ImageUrl" IS NULL;

-- GameEvents table
UPDATE "GameEvents" SET "ImageUrl" = '/public/images/somtam-zing-logo.png' WHERE "ImageUrl" IS NULL;

-- Achievements table (entity: GameAchievement)
UPDATE "Achievements" SET "ImageUrl" = '/public/images/somtam-zing-logo.png' WHERE "ImageUrl" IS NULL;

-- PlayerCosmetics table
UPDATE "PlayerCosmetics" SET "ImageUrl" = '/public/images/somtam-zing-logo.png' WHERE "ImageUrl" IS NULL;

-- Missions table
UPDATE "Missions" SET "ImageUrl" = '/public/images/somtam-zing-logo.png' WHERE "ImageUrl" IS NULL;

-- PlayerIngredients table
UPDATE "PlayerIngredients" SET "ImageUrl" = '/public/images/somtam-zing-logo.png' WHERE "ImageUrl" IS NULL;

-- Users table (AvatarUrl)
UPDATE "Users" SET "AvatarUrl" = '/public/images/somtam-zing-logo.png' WHERE "AvatarUrl" IS NULL;