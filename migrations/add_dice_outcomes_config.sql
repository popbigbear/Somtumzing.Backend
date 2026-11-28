-- Migration: Add dice outcomes config to GameConfigs table
-- Generated: 2026-11-28

-- Add new columns to GameConfigs table
ALTER TABLE "GameConfigs" 
ADD COLUMN IF NOT EXISTS "DiceWinCoins" INT4 DEFAULT 25,
ADD COLUMN IF NOT EXISTS "DiceLossCoins" INT4 DEFAULT -10,
ADD COLUMN IF NOT EXISTS "DiceWinPowerCard" BOOLEAN DEFAULT TRUE;

-- Update existing records with default values
UPDATE "GameConfigs" 
SET 
    "DiceWinCoins" = 25,
    "DiceLossCoins" = -10,
    "DiceWinPowerCard" = TRUE
WHERE "DiceWinCoins" IS NULL;

-- Rollback (if needed)
-- ALTER TABLE "GameConfigs" 
-- DROP COLUMN IF EXISTS "DiceWinCoins",
-- DROP COLUMN IF EXISTS "DiceLossCoins",
-- DROP COLUMN IF EXISTS "DiceWinPowerCard";