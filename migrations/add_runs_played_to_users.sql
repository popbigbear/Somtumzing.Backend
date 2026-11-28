-- Migration: Add RunsPlayed column to Users table
-- Generated: 2026-11-28

-- Add RunsPlayed column to Users table
ALTER TABLE "Users" 
ADD COLUMN IF NOT EXISTS "RunsPlayed" INT4 DEFAULT 0;

-- Update existing records with default value
UPDATE "Users" SET "RunsPlayed" = 0 WHERE "RunsPlayed" IS NULL;

-- Rollback (if needed)
-- ALTER TABLE "Users" DROP COLUMN IF EXISTS "RunsPlayed";