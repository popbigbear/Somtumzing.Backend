-- Migration: Add threshold config to GameConfigs table
-- Generated: 2026-11-28

-- Add new columns to GameConfigs table
ALTER TABLE "GameConfigs" 
ADD COLUMN IF NOT EXISTS "QueueCapacity" INT4 DEFAULT 5,
ADD COLUMN IF NOT EXISTS "UrgentThreshold" INT4 DEFAULT 4,
ADD COLUMN IF NOT EXISTS "WarningTimeSec" INT4 DEFAULT 10,
ADD COLUMN IF NOT EXISTS "UrgentSlotBase" INT4 DEFAULT 1,
ADD COLUMN IF NOT EXISTS "UrgentSlotIntervalSec" INT4 DEFAULT 10;

-- Update existing records with default values
UPDATE "GameConfigs" 
SET 
    "QueueCapacity" = 5,
    "UrgentThreshold" = 4,
    "WarningTimeSec" = 10,
    "UrgentSlotBase" = 1,
    "UrgentSlotIntervalSec" = 10
WHERE "QueueCapacity" IS NULL;

-- Rollback (if needed)
-- ALTER TABLE "GameConfigs" 
-- DROP COLUMN IF EXISTS "QueueCapacity",
-- DROP COLUMN IF EXISTS "UrgentThreshold",
-- DROP COLUMN IF EXISTS "WarningTimeSec",
-- DROP COLUMN IF EXISTS "UrgentSlotBase",
-- DROP COLUMN IF EXISTS "UrgentSlotIntervalSec";