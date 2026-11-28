-- Migration: Add new game-specific fields to GameConfigs table
-- Generated: 2026-11-27

-- Add new columns to GameConfigs table
ALTER TABLE "GameConfigs" 
ADD COLUMN IF NOT EXISTS "Customers" TEXT[] DEFAULT ARRAY[]::TEXT[],
ADD COLUMN IF NOT EXISTS "RunSeconds" INT4 DEFAULT 180,
ADD COLUMN IF NOT EXISTS "MaxPerIngredient" INT4 DEFAULT 10,
ADD COLUMN IF NOT EXISTS "MaxWarnings" INT4 DEFAULT 3,
ADD COLUMN IF NOT EXISTS "PaotungScansRequired" INT4 DEFAULT 3,
ADD COLUMN IF NOT EXISTS "TessakitFine" INT4 DEFAULT 50,
ADD COLUMN IF NOT EXISTS "PaotungChance" INT4 DEFAULT 20,
ADD COLUMN IF NOT EXISTS "PaotungBonus" INT4 DEFAULT 100,
ADD COLUMN IF NOT EXISTS "PaotungFailChance" INT4 DEFAULT 10,
ADD COLUMN IF NOT EXISTS "TessakitChancePerSec" INT4 DEFAULT 5,
ADD COLUMN IF NOT EXISTS "TessakitWarningSec" INT4 DEFAULT 10;

-- Update existing records with default values
UPDATE "GameConfigs" 
SET 
    "Customers" = ARRAY[]::TEXT[],
    "RunSeconds" = 180,
    "MaxPerIngredient" = 10,
    "MaxWarnings" = 3,
    "PaotungScansRequired" = 3,
    "TessakitFine" = 50,
    "PaotungChance" = 20,
    "PaotungBonus" = 100,
    "PaotungFailChance" = 10,
    "TessakitChancePerSec" = 5,
    "TessakitWarningSec" = 10
WHERE "Customers" IS NULL;

-- Rollback (if needed)
-- ALTER TABLE "GameConfigs" 
-- DROP COLUMN IF EXISTS "Customers",
-- DROP COLUMN IF EXISTS "RunSeconds",
-- DROP COLUMN IF EXISTS "MaxPerIngredient",
-- DROP COLUMN IF EXISTS "MaxWarnings",
-- DROP COLUMN IF EXISTS "PaotungScansRequired",
-- DROP COLUMN IF EXISTS "TessakitFine",
-- DROP COLUMN IF EXISTS "PaotungChance",
-- DROP COLUMN IF EXISTS "PaotungBonus",
-- DROP COLUMN IF EXISTS "PaotungFailChance",
-- DROP COLUMN IF EXISTS "TessakitChancePerSec",
-- DROP COLUMN IF EXISTS "TessakitWarningSec";