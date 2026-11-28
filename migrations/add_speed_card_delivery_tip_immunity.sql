-- Migration: Add speed card, delivery tip, immunity config to GameConfigs table
-- Generated: 2026-11-28

-- Add new columns to GameConfigs table
ALTER TABLE "GameConfigs" 
ADD COLUMN IF NOT EXISTS "SpeedCardDurationSec" INT4 DEFAULT 12,
ADD COLUMN IF NOT EXISTS "SpeedCardBonusCoins" INT4 DEFAULT 10,
ADD COLUMN IF NOT EXISTS "DeliveryTipCoins" INT4 DEFAULT 15,
ADD COLUMN IF NOT EXISTS "FleeImmunitySec" INT4 DEFAULT 6;

-- Update existing records with default values
UPDATE "GameConfigs" 
SET 
    "SpeedCardDurationSec" = 12,
    "SpeedCardBonusCoins" = 10,
    "DeliveryTipCoins" = 15,
    "FleeImmunitySec" = 6
WHERE "SpeedCardDurationSec" IS NULL;

-- Rollback (if needed)
-- ALTER TABLE "GameConfigs" 
-- DROP COLUMN IF EXISTS "SpeedCardDurationSec",
-- DROP COLUMN IF EXISTS "SpeedCardBonusCoins",
-- DROP COLUMN IF EXISTS "DeliveryTipCoins",
-- DROP COLUMN IF EXISTS "FleeImmunitySec";