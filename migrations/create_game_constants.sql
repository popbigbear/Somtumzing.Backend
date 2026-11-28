-- Migration: Create GameConstants table for game constants
-- Generated: 2026-11-27

-- Create GameConstants table
CREATE TABLE "GameConstants" (
    "PK" UUID NOT NULL DEFAULT gen_random_uuid(),
    "Code" VARCHAR(40) NOT NULL,
    "CreatedDate" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID NULL,
    "UpdatedDate" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" UUID NULL,
    "Status" VARCHAR(24) NOT NULL DEFAULT 'ACTIVE',
    
    "ConstantKey" VARCHAR(64) NOT NULL,
    "Category" VARCHAR(32) NOT NULL,
    "DisplayName" VARCHAR(160) NOT NULL,
    "Description" VARCHAR(500) NULL,
    "ValueJson" TEXT NOT NULL DEFAULT '{}',
    "SortOrder" INT4 NOT NULL DEFAULT 0,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT "PK_GameConstants" PRIMARY KEY ("PK"),
    CONSTRAINT "CK_GameConstants_Status" CHECK ("Status" IN ('ACTIVE', 'INACTIVE')),
    CONSTRAINT "CK_GameConstants_Category" CHECK ("Category" IN ('DICE', 'SPEED_CARD', 'DELIVERY_TIP', 'IMMUNITY', 'OTHER'))
);

-- Create unique index on ConstantKey
CREATE UNIQUE INDEX "IX_GameConstants_ConstantKey" ON "GameConstants" ("ConstantKey");

-- Create index on Category
CREATE INDEX "IX_GameConstants_Category" ON "GameConstants" ("Category");

-- Create index on Status + IsActive
CREATE INDEX "IX_GameConstants_Status_IsActive" ON "GameConstants" ("Status", "IsActive");

-- Insert default game constants

-- 1. DICE OUTCOMES
INSERT INTO "GameConstants" ("Code", "ConstantKey", "Category", "DisplayName", "Description", "ValueJson", "SortOrder", "IsActive") VALUES
('CONST_001', 'DICE_FACES', 'DICE', 'Dice Faces', 'Number of faces on the dice', '{"faces": 6}', 1, TRUE),
('CONST_002', 'DICE_SUCCESS_THRESHOLD', 'DICE', 'Success Threshold', 'Minimum dice value for success', '{"minValue": 4}', 2, TRUE),
('CONST_003', 'DICE_CRITICAL_VALUE', 'DICE', 'Critical Success Value', 'Dice value for critical success', '{"value": 6}', 3, TRUE),
('CONST_004', 'DICE_CRITICAL_MULTIPLIER', 'DICE', 'Critical Multiplier', 'Score multiplier on critical', '{"multiplier": 2.0}', 4, TRUE),
('CONST_005', 'DICE_ROLL_COOLDOWN_MS', 'DICE', 'Roll Cooldown', 'Minimum milliseconds between rolls', '{"cooldownMs": 500}', 5, TRUE),
('CONST_006', 'DICE_REROLL_COST', 'DICE', 'Reroll Cost', 'Coins cost to reroll dice', '{"cost": 10}', 6, TRUE);

-- 2. SPEED CARD DURATION/BONUS
INSERT INTO "GameConstants" ("Code", "ConstantKey", "Category", "DisplayName", "Description", "ValueJson", "SortOrder", "IsActive") VALUES
('CONST_011', 'SPEED_CARD_DURATION_SEC', 'SPEED_CARD', 'Speed Card Duration', 'Duration of speed boost in seconds', '{"durationSeconds": 30}', 1, TRUE),
('CONST_012', 'SPEED_CARD_SPEED_MULTIPLIER', 'SPEED_CARD', 'Speed Multiplier', 'Movement speed multiplier', '{"multiplier": 1.5}', 2, TRUE),
('CONST_013', 'SPEED_CARD_COOLDOWN_SEC', 'SPEED_CARD', 'Speed Card Cooldown', 'Cooldown between speed card uses', '{"cooldownSeconds': 60}', 3, TRUE),
('CONST_014', 'SPEED_CARD_MAX_STACKS', 'SPEED_CARD', 'Max Stacks', 'Maximum number of speed card stacks', '{"maxStacks": 3}', 4, TRUE),
('CONST_015', 'SPEED_CARD_BONUS_POINTS', 'SPEED_CARD', 'Bonus Points', 'Bonus points per action during speed', '{"bonusPoints": 5}', 5, TRUE);

-- 3. DELIVERY TIP
INSERT INTO "GameConstants" ("Code", "ConstantKey", "Category", "DisplayName", "Description", "ValueJson", "SortOrder", "IsActive") VALUES
('CONST_021', 'DELIVERY_TIP_BASE', 'DELIVERY_TIP', 'Base Tip', 'Base tip amount for delivery', '{"baseTip": 5}', 1, TRUE),
('CONST_022', 'DELIVERY_TIP_SPEED_BONUS', 'DELIVERY_TIP', 'Speed Bonus Tip', 'Extra tip for fast delivery', '{"speedBonus": 10}', 2, TRUE),
('CONST_023', 'DELIVERY_TIP_PERFECT_BONUS', 'DELIVERY_TIP', 'Perfect Delivery Bonus', 'Extra tip for perfect delivery', '{"perfectBonus": 15}', 3, TRUE),
('CONST_024', 'DELIVERY_TIP_DISTANCE_MULTIPLIER', 'DELIVERY_TIP', 'Distance Multiplier', 'Tip multiplier based on distance', '{"multiplier": 1.2}', 4, TRUE),
('CONST_025', 'DELIVERY_TIP_MAX', 'DELIVERY_TIP', 'Max Tip', 'Maximum tip per delivery', '{"maxTip": 100}', 5, TRUE);

-- 4. IMMUNITY DURATION, THRESHOLDS
INSERT INTO "GameConstants" ("Code", "ConstantKey", "Category", "DisplayName", "Description", "ValueJson", "SortOrder", "IsActive") VALUES
('CONST_031', 'IMMUNITY_DURATION_SEC', 'IMMUNITY', 'Immunity Duration', 'Duration of immunity shield in seconds', '{"durationSeconds": 10}', 1, TRUE),
('CONST_032', 'IMMUNITY_COOLDOWN_SEC', 'IMMUNITY', 'Immunity Cooldown', 'Cooldown before immunity can be used again', '{"cooldownSeconds': 120}', 2, TRUE),
('CONST_033', 'IMMUNITY_ACTIVATION_THRESHOLD', 'IMMUNITY', 'Activation Threshold', 'HP/Warning threshold to trigger immunity', '{"threshold": 3}', 3, TRUE),
('CONST_034', 'IMMUNITY_MAX_DURATION_SEC', 'IMMUNITY', 'Max Immunity Duration', 'Maximum extended immunity duration', '{"maxDurationSeconds': 30}', 4, TRUE),
('CONST_035', 'IMMUNITY_REFRESH_ON_CRITICAL', 'IMMUNITY', 'Refresh on Critical', 'Reset immunity cooldown on critical hit', '{"enabled": true}', 5, TRUE),
('CONST_036', 'IMMUNITY_VISUAL_EFFECT', 'IMMUNITY', 'Visual Effect', 'Show visual effect during immunity', '{"enabled": true, "color": "#00FFFF"}', 6, TRUE);

-- Rollback (if needed)
-- DROP TABLE IF EXISTS "GameConstants";