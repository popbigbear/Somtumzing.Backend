-- Migration: Add TablesJson column to GameConfigs table
-- Generated: 2026-11-28

-- Add TablesJson column to GameConfigs table
ALTER TABLE "GameConfigs" 
ADD COLUMN IF NOT EXISTS "TablesJson" TEXT DEFAULT '[]';

-- Update existing records with default tables configuration
UPDATE "GameConfigs" 
SET "TablesJson" = '[
  {"id": 1, "label": "โต๊ะ 1", "customer": "ป้าแดง", "status": "urgent", "orders": 1},
  {"id": 2, "label": "โต๊ะ 2", "customer": "พี่วินหน้าซอย", "status": "waiting", "orders": 2},
  {"id": 3, "label": "โต๊ะ 3", "customer": "น้องออฟฟิศ", "status": "waiting", "orders": 1},
  {"id": 4, "label": "โต๊ะ 4", "customer": "เจ๊ขายหวย", "status": "waiting", "orders": 3}
]'::jsonb
WHERE "TablesJson" = '[]' OR "TablesJson" IS NULL;

-- Rollback (if needed)
-- ALTER TABLE "GameConfigs" DROP COLUMN IF EXISTS "TablesJson";