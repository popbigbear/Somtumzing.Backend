-- Migration: Add tableCount, deliveryOrders, maxPowerCards to GameConfigs table
-- Generated: 2026-11-28

-- Add new columns to GameConfigs table
ALTER TABLE "GameConfigs" 
ADD COLUMN IF NOT EXISTS "TableCount" INT4 DEFAULT 4,
ADD COLUMN IF NOT EXISTS "DeliveryOrders" INT4 DEFAULT 2,
ADD COLUMN IF NOT EXISTS "MaxPowerCards" INT4 DEFAULT 2;

-- Update existing records with default values
UPDATE "GameConfigs" 
SET 
    "TableCount" = 4,
    "DeliveryOrders" = 2,
    "MaxPowerCards" = 2
WHERE "TableCount" IS NULL;

-- Rollback (if needed)
-- ALTER TABLE "GameConfigs" 
-- DROP COLUMN IF EXISTS "TableCount",
-- DROP COLUMN IF EXISTS "DeliveryOrders",
-- DROP COLUMN IF EXISTS "MaxPowerCards";