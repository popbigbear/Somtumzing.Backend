-- Migration: Add DiceRoundsJson column to GameConfigs table
-- Generated: 2026-11-28

-- Add DiceRoundsJson column to GameConfigs table
ALTER TABLE "GameConfigs" 
ADD COLUMN IF NOT EXISTS "DiceRoundsJson" TEXT DEFAULT '[]';

-- Update existing records with default dice rounds configuration
UPDATE "GameConfigs" 
SET "DiceRoundsJson" = '[
  {"label": "เช้า", "choice": null, "dice": [], "total": null, "delta": null},
  {"label": "กลางวัน", "choice": null, "dice": [], "total": null, "delta": null},
  {"label": "เย็น", "choice": null, "dice": [], "total": null, "delta": null}
]'::jsonb
WHERE "DiceRoundsJson" = '[]' OR "DiceRoundsJson" IS NULL;

-- Rollback (if needed)
-- ALTER TABLE "GameConfigs" DROP COLUMN IF EXISTS "DiceRoundsJson";