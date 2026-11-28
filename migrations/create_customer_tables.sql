-- Migration: Create CustomerTables table for customer-table config
-- Generated: 2026-11-27

-- Create CustomerTables table
CREATE TABLE "CustomerTables" (
    "PK" UUID NOT NULL DEFAULT gen_random_uuid(),
    "Code" VARCHAR(40) NOT NULL,
    "CreatedDate" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID NULL,
    "UpdatedDate" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" UUID NULL,
    "Status" VARCHAR(24) NOT NULL DEFAULT 'ACTIVE',
    
    "TableNumber" INT4 NOT NULL,
    "Capacity" INT4 NOT NULL DEFAULT 4,
    "InitialStatus" VARCHAR(24) NOT NULL DEFAULT 'EMPTY',
    "AssignedCustomerType" VARCHAR(32) NULL,
    "AssignedCustomerId" INT4 NULL,
    "PositionX" INT4 NOT NULL DEFAULT 0,
    "PositionY" INT4 NOT NULL DEFAULT 0,
    "Zone" VARCHAR(32) NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "Notes" VARCHAR(500) NULL,
    
    CONSTRAINT "PK_CustomerTables" PRIMARY KEY ("PK"),
    CONSTRAINT "CK_CustomerTables_Status" CHECK ("Status" IN ('ACTIVE', 'INACTIVE')),
    CONSTRAINT "CK_CustomerTables_InitialStatus" CHECK ("InitialStatus" IN ('EMPTY', 'OCCUPIED', 'RESERVED', 'DIRTY')),
    CONSTRAINT "CK_CustomerTables_Zone" CHECK ("Zone" IS NULL OR "Zone" IN ('INDOOR', 'OUTDOOR', 'VIP', 'DEFAULT')),
    CONSTRAINT "CK_CustomerTables_AssignedCustomerType" CHECK ("AssignedCustomerType" IS NULL OR "AssignedCustomerType" IN ('REGULAR', 'VIP', 'PAOTUNG', 'TESSAKIT'))
);

-- Create unique index on TableNumber
CREATE UNIQUE INDEX "IX_CustomerTables_TableNumber" ON "CustomerTables" ("TableNumber");

-- Create indexes for common queries
CREATE INDEX "IX_CustomerTables_Status_IsActive" ON "CustomerTables" ("Status", "IsActive");
CREATE INDEX "IX_CustomerTables_Zone" ON "CustomerTables" ("Zone");
CREATE INDEX "IX_CustomerTables_AssignedCustomerType" ON "CustomerTables" ("AssignedCustomerType");

-- Foreign keys for CreatedBy/UpdatedBy (optional - reference Users table)
-- ALTER TABLE "CustomerTables" 
-- ADD CONSTRAINT "FK_CustomerTables_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "Users" ("Id") ON DELETE RESTRICT,
-- ADD CONSTRAINT "FK_CustomerTables_UpdatedBy" FOREIGN KEY ("UpdatedBy") REFERENCES "Users" ("Id") ON DELETE RESTRICT;

-- Insert default customer tables (example: 10 tables)
INSERT INTO "CustomerTables" ("Code", "TableNumber", "Capacity", "InitialStatus", "Zone", "IsActive", "Notes") VALUES
('TBL_0001', 1, 4, 'EMPTY', 'INDOOR', TRUE, 'Table 1 - Indoor'),
('TBL_0002', 2, 4, 'EMPTY', 'INDOOR', TRUE, 'Table 2 - Indoor'),
('TBL_0003', 3, 4, 'EMPTY', 'INDOOR', TRUE, 'Table 3 - Indoor'),
('TBL_0004', 4, 2, 'EMPTY', 'OUTDOOR', TRUE, 'Table 4 - Outdoor (2 seats)'),
('TBL_0005', 5, 2, 'EMPTY', 'OUTDOOR', TRUE, 'Table 5 - Outdoor (2 seats)'),
('TBL_0006', 6, 6, 'EMPTY', 'INDOOR', TRUE, 'Table 6 - Large Indoor'),
('TBL_0007', 7, 4, 'EMPTY', 'VIP', TRUE, 'Table 7 - VIP Zone'),
('TBL_0008', 8, 4, 'EMPTY', 'VIP', TRUE, 'Table 8 - VIP Zone'),
('TBL_0009', 9, 8, 'EMPTY', 'INDOOR', TRUE, 'Table 9 - Large Indoor'),
('TBL_0010', 10, 2, 'EMPTY', 'OUTDOOR', TRUE, 'Table 10 - Outdoor (2 seats)');

-- Rollback (if needed)
-- DROP TABLE IF EXISTS "CustomerTables";