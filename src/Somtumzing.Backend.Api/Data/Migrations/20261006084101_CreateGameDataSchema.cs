using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Somtumzing.Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateGameDataSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RefreshTokens",
                newName: "UserPK");

            migrationBuilder.RenameColumn(
                name: "RevokedAtUtc",
                table: "RefreshTokens",
                newName: "RevokedDate");

            migrationBuilder.RenameColumn(
                name: "ReplacedByTokenId",
                table: "RefreshTokens",
                newName: "ReplacedByPK");

            migrationBuilder.RenameColumn(
                name: "ExpiresAtUtc",
                table: "RefreshTokens",
                newName: "ExpiresDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "RefreshTokens",
                newName: "CreatedTokenDate");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserPK");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "AspNetUserRoles",
                newName: "RolePK");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AspNetUserRoles",
                newName: "UserPK");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RolePK");

            migrationBuilder.RenameColumn(
                name: "SuspendedByAdminId",
                table: "Users",
                newName: "SuspendedByPK");

            migrationBuilder.RenameColumn(
                name: "SuspendedAtUtc",
                table: "Users",
                newName: "SuspendedDate");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "RefreshTokens",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "RefreshTokens",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "RefreshTokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "RefreshTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RefreshTokens",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "ACTIVE");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "RefreshTokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "RefreshTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AuditLogEntries",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AuditLogEntries",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AuditLogEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "AuditLogEntries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AuditLogEntries",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "RECORDED");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "AuditLogEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "AuditLogEntries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "PK",
                table: "AspNetUserRoles",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AspNetUserRoles",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AspNetUserRoles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "AspNetUserRoles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AspNetUserRoles",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "ACTIVE");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "AspNetUserRoles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "AspNetUserRoles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AspNetRoles",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AspNetRoles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AspNetRoles",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "ACTIVE");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "AspNetRoles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "AspNetRoles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Users",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "ACTIVE");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                column: "PK");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChaosEventDefinitions",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    EventKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    TriggerConfig = table.Column<string>(type: "jsonb", nullable: false),
                    ResponseWindowSeconds = table.Column<decimal>(type: "numeric(7,3)", precision: 7, scale: 3, nullable: true),
                    AllowedActions = table.Column<string>(type: "jsonb", nullable: false),
                    FailureEffects = table.Column<string>(type: "jsonb", nullable: false),
                    ActiveEffects = table.Column<string>(type: "jsonb", nullable: false),
                    RulesetKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChaosEventDefinitions", x => x.PK);
                    table.CheckConstraint("CK_ChaosEventDefinitions_Status", "\"Status\" IN ('DRAFT', 'PUBLISHED', 'RETIRED')");
                    table.ForeignKey(
                        name: "FK_ChaosEventDefinitions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChaosEventDefinitions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CodeCounters",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    PeriodYYMM = table.Column<string>(type: "character(4)", fixedLength: true, maxLength: 4, nullable: false),
                    TableCode = table.Column<string>(type: "character(1)", fixedLength: true, maxLength: 1, nullable: false),
                    NextNumber = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeCounters", x => x.PK);
                    table.CheckConstraint("CK_CodeCounters_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_CodeCounters_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodeCounters_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdempotencyRecords",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ActorUserPK = table.Column<Guid>(type: "uuid", nullable: true),
                    Scope = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdempotencyKey = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestHash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResourcePK = table.Column<Guid>(type: "uuid", nullable: true),
                    ResponseStatus = table.Column<short>(type: "smallint", nullable: true),
                    ResponseBody = table.Column<string>(type: "jsonb", nullable: true),
                    ExpiresDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyRecords", x => x.PK);
                    table.CheckConstraint("CK_IdempotencyRecords_Status", "\"Status\" IN ('IN_PROGRESS', 'COMPLETED', 'FAILED')");
                    table.ForeignKey(
                        name: "FK_IdempotencyRecords_Users_ActorUserPK",
                        column: x => x.ActorUserPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdempotencyRecords_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdempotencyRecords_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Mode = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    RulesetKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SeedHash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    StartedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpectedEndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    WinnerTeam = table.Column<short>(type: "smallint", nullable: true),
                    ResultCode = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.PK);
                    table.CheckConstraint("CK_Matches_Status", "\"Status\" IN ('FORMING', 'READY', 'IN_PROGRESS', 'COMPLETED', 'FORFEIT', 'INVALID', 'CANCELLED')");
                    table.ForeignKey(
                        name: "FK_Matches_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerLoadouts",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UserPK = table.Column<Guid>(type: "uuid", nullable: false),
                    RulesetKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLoadouts", x => x.PK);
                    table.CheckConstraint("CK_PlayerLoadouts_Status", "\"Status\" IN ('ACTIVE', 'INACTIVE')");
                    table.ForeignKey(
                        name: "FK_PlayerLoadouts_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerLoadouts_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerLoadouts_Users_UserPK",
                        column: x => x.UserPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SabotageCards",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    CardKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Tier = table.Column<short>(type: "smallint", nullable: true),
                    EnergyCostPct = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    EffectDurationSeconds = table.Column<decimal>(type: "numeric(7,3)", precision: 7, scale: 3, nullable: false),
                    CounterActionCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EffectConfig = table.Column<string>(type: "jsonb", nullable: false),
                    RulesetKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SabotageCards", x => x.PK);
                    table.CheckConstraint("CK_SabotageCards_Status", "\"Status\" IN ('DRAFT', 'PUBLISHED', 'RETIRED')");
                    table.ForeignKey(
                        name: "FK_SabotageCards_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SabotageCards_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stages",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    StageKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    RequiredUnlockStars = table.Column<int>(type: "integer", nullable: false),
                    DefaultConfigVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.PK);
                    table.CheckConstraint("CK_Stages_Status", "\"Status\" IN ('DRAFT', 'PUBLISHED', 'RETIRED')");
                    table.ForeignKey(
                        name: "FK_Stages_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stages_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Upgrades",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UpgradeKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(20,4)", precision: 20, scale: 4, nullable: false),
                    MaxLevel = table.Column<int>(type: "integer", nullable: false),
                    EffectConfig = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Upgrades", x => x.PK);
                    table.CheckConstraint("CK_Upgrades_Status", "\"Status\" IN ('DRAFT', 'PUBLISHED', 'RETIRED')");
                    table.ForeignKey(
                        name: "FK_Upgrades_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Upgrades_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UserPK = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(20,4)", precision: 20, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.PK);
                    table.CheckConstraint("CK_Wallets_Status", "\"Status\" IN ('ACTIVE', 'FROZEN', 'CLOSED')");
                    table.ForeignKey(
                        name: "FK_Wallets_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wallets_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Wallets_Users_UserPK",
                        column: x => x.UserPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchParticipants",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    MatchPK = table.Column<Guid>(type: "uuid", nullable: false),
                    UserPK = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamNumber = table.Column<short>(type: "smallint", nullable: false),
                    RatingBefore = table.Column<int>(type: "integer", nullable: true),
                    RatingAfter = table.Column<int>(type: "integer", nullable: true),
                    JoinedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DisconnectedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeckSnapshot = table.Column<string>(type: "jsonb", nullable: true),
                    DefenseCardCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchParticipants", x => x.PK);
                    table.CheckConstraint("CK_MatchParticipants_Status", "\"Status\" IN ('ACTIVE', 'DISCONNECTED', 'FORFEIT', 'COMPLETED')");
                    table.ForeignKey(
                        name: "FK_MatchParticipants_Matches_MatchPK",
                        column: x => x.MatchPK,
                        principalTable: "Matches",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchParticipants_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchParticipants_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchParticipants_Users_UserPK",
                        column: x => x.UserPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerLoadoutCards",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    LoadoutPK = table.Column<Guid>(type: "uuid", nullable: false),
                    CardPK = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    SlotNumber = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLoadoutCards", x => x.PK);
                    table.CheckConstraint("CK_PlayerLoadoutCards_Status", "\"Status\" IN ('ACTIVE', 'REMOVED')");
                    table.ForeignKey(
                        name: "FK_PlayerLoadoutCards_PlayerLoadouts_LoadoutPK",
                        column: x => x.LoadoutPK,
                        principalTable: "PlayerLoadouts",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerLoadoutCards_SabotageCards_CardPK",
                        column: x => x.CardPK,
                        principalTable: "SabotageCards",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerLoadoutCards_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerLoadoutCards_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStageProgress",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UserPK = table.Column<Guid>(type: "uuid", nullable: false),
                    StagePK = table.Column<Guid>(type: "uuid", nullable: false),
                    BestStars = table.Column<int>(type: "integer", nullable: false),
                    TotalStars = table.Column<int>(type: "integer", nullable: false),
                    BestNetProfit = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    FirstCompletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastPlayedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStageProgress", x => x.PK);
                    table.CheckConstraint("CK_PlayerStageProgress_NonNegative", "\"BestStars\" >= 0 AND \"TotalStars\" >= 0");
                    table.CheckConstraint("CK_PlayerStageProgress_Status", "\"Status\" IN ('LOCKED', 'UNLOCKED', 'COMPLETED')");
                    table.ForeignKey(
                        name: "FK_PlayerStageProgress_Stages_StagePK",
                        column: x => x.StagePK,
                        principalTable: "Stages",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerStageProgress_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerStageProgress_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerStageProgress_Users_UserPK",
                        column: x => x.UserPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StageConfigs",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    StagePK = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigVersion = table.Column<int>(type: "integer", nullable: false),
                    RulesetKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    TargetSalesAmount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    MaxTableCapacity = table.Column<int>(type: "integer", nullable: false),
                    DeliveryQueueLimit = table.Column<int>(type: "integer", nullable: false),
                    RawMaterialCostRate = table.Column<decimal>(type: "numeric(7,6)", precision: 7, scale: 6, nullable: true),
                    ChaosConfig = table.Column<string>(type: "jsonb", nullable: false),
                    PublishedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageConfigs", x => x.PK);
                    table.UniqueConstraint("AK_StageConfigs_StagePK_ConfigVersion", x => new { x.StagePK, x.ConfigVersion });
                    table.CheckConstraint("CK_StageConfigs_Status", "\"Status\" IN ('DRAFT', 'PUBLISHED', 'RETIRED')");
                    table.ForeignKey(
                        name: "FK_StageConfigs_Stages_StagePK",
                        column: x => x.StagePK,
                        principalTable: "Stages",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StageConfigs_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StageConfigs_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerUpgrades",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UserPK = table.Column<Guid>(type: "uuid", nullable: false),
                    UpgradePK = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    PurchasedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUpgrades", x => x.PK);
                    table.CheckConstraint("CK_PlayerUpgrades_Status", "\"Status\" IN ('ACTIVE', 'INACTIVE')");
                    table.ForeignKey(
                        name: "FK_PlayerUpgrades_Upgrades_UpgradePK",
                        column: x => x.UpgradePK,
                        principalTable: "Upgrades",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerUpgrades_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerUpgrades_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerUpgrades_Users_UserPK",
                        column: x => x.UserPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    WalletPK = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(20,4)", precision: 20, scale: 4, nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "numeric(20,4)", precision: 20, scale: 4, nullable: false),
                    ReferenceType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ReferencePK = table.Column<Guid>(type: "uuid", nullable: true),
                    ReferenceCode = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ReversalOfPK = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.PK);
                    table.CheckConstraint("CK_WalletTransactions_Status", "\"Status\" IN ('POSTED', 'REVERSED')");
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_WalletTransactions_ReversalOfPK",
                        column: x => x.ReversalOfPK,
                        principalTable: "WalletTransactions",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_WalletPK",
                        column: x => x.WalletPK,
                        principalTable: "Wallets",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchActions",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    MatchPK = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantPK = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    ActionCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TargetCode = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    ActionData = table.Column<string>(type: "jsonb", nullable: false),
                    ServerReceivedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ClientOccurredDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false),
                    RejectCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    MatchVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchActions", x => x.PK);
                    table.CheckConstraint("CK_MatchActions_Status", "\"Status\" IN ('ACCEPTED', 'REJECTED')");
                    table.ForeignKey(
                        name: "FK_MatchActions_MatchParticipants_ParticipantPK",
                        column: x => x.ParticipantPK,
                        principalTable: "MatchParticipants",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchActions_Matches_MatchPK",
                        column: x => x.MatchPK,
                        principalTable: "Matches",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchActions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchActions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchResults",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    MatchPK = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantPK = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamNumber = table.Column<short>(type: "smallint", nullable: false),
                    GrossRevenue = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    NetProfit = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    EarnedStars = table.Column<int>(type: "integer", nullable: false),
                    Outcome = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    RatingChange = table.Column<int>(type: "integer", nullable: true),
                    FinalizedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchResults", x => x.PK);
                    table.CheckConstraint("CK_MatchResults_Status", "\"Status\" IN ('FINAL', 'INVALIDATED')");
                    table.ForeignKey(
                        name: "FK_MatchResults_MatchParticipants_ParticipantPK",
                        column: x => x.ParticipantPK,
                        principalTable: "MatchParticipants",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchResults_Matches_MatchPK",
                        column: x => x.MatchPK,
                        principalTable: "Matches",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchResults_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchResults_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchSabotageEvents",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    MatchPK = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceParticipantPK = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetParticipantPK = table.Column<Guid>(type: "uuid", nullable: false),
                    CardPK = table.Column<Guid>(type: "uuid", nullable: false),
                    EventSequence = table.Column<long>(type: "bigint", nullable: false),
                    TriggeredDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RespondByDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResolvedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CounterActionCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResultCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    EnergySpentPct = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    EffectSnapshot = table.Column<string>(type: "jsonb", nullable: false),
                    ImmunityUntilDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchSabotageEvents", x => x.PK);
                    table.CheckConstraint("CK_MatchSabotageEvents_Status", "\"Status\" IN ('WARNING', 'ACTIVE', 'COUNTERED', 'FAILED', 'EXPIRED', 'BLOCKED')");
                    table.ForeignKey(
                        name: "FK_MatchSabotageEvents_MatchParticipants_SourceParticipantPK",
                        column: x => x.SourceParticipantPK,
                        principalTable: "MatchParticipants",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchSabotageEvents_MatchParticipants_TargetParticipantPK",
                        column: x => x.TargetParticipantPK,
                        principalTable: "MatchParticipants",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchSabotageEvents_Matches_MatchPK",
                        column: x => x.MatchPK,
                        principalTable: "Matches",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchSabotageEvents_SabotageCards_CardPK",
                        column: x => x.CardPK,
                        principalTable: "SabotageCards",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchSabotageEvents_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchSabotageEvents_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ChallengeKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    StagePK = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigVersion = table.Column<int>(type: "integer", nullable: false),
                    RulesetKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StartsDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndsDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AttemptLimit = table.Column<int>(type: "integer", nullable: true),
                    RewardConfig = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.PK);
                    table.CheckConstraint("CK_Challenges_Status", "\"Status\" IN ('DRAFT', 'SCHEDULED', 'ACTIVE', 'CLOSED', 'CANCELLED')");
                    table.ForeignKey(
                        name: "FK_Challenges_StageConfigs_StagePK_ConfigVersion",
                        columns: x => new { x.StagePK, x.ConfigVersion },
                        principalTable: "StageConfigs",
                        principalColumns: new[] { "StagePK", "ConfigVersion" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Challenges_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Challenges_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UserPK = table.Column<Guid>(type: "uuid", nullable: false),
                    StagePK = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigVersion = table.Column<int>(type: "integer", nullable: false),
                    RulesetKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SessionType = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    StartedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpectedEndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SettledDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ServerSeed = table.Column<byte[]>(type: "bytea", nullable: true),
                    SessionVersion = table.Column<long>(type: "bigint", nullable: false),
                    GrossRevenue = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    NetProfit = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    EarnedStars = table.Column<int>(type: "integer", nullable: true),
                    DailyRating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.PK);
                    table.CheckConstraint("CK_GameSessions_Status", "\"Status\" IN ('ACTIVE', 'COMPLETED', 'SETTLED', 'ABANDONED', 'INVALID')");
                    table.ForeignKey(
                        name: "FK_GameSessions_StageConfigs_StagePK_ConfigVersion",
                        columns: x => new { x.StagePK, x.ConfigVersion },
                        principalTable: "StageConfigs",
                        principalColumns: new[] { "StagePK", "ConfigVersion" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameSessions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameSessions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameSessions_Users_UserPK",
                        column: x => x.UserPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeRuns",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ChallengePK = table.Column<Guid>(type: "uuid", nullable: false),
                    UserPK = table.Column<Guid>(type: "uuid", nullable: false),
                    RunSequence = table.Column<int>(type: "integer", nullable: false),
                    StartedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SubmittedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RunTicketHash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    EventEvidence = table.Column<string>(type: "jsonb", nullable: true),
                    CalculatedScore = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    CalculatedNetProfit = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    CalculatedStars = table.Column<int>(type: "integer", nullable: true),
                    ValidationCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    RankSnapshot = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeRuns", x => x.PK);
                    table.CheckConstraint("CK_ChallengeRuns_Status", "\"Status\" IN ('ACTIVE', 'SUBMITTED', 'VALIDATING', 'ACCEPTED', 'REJECTED', 'EXPIRED')");
                    table.ForeignKey(
                        name: "FK_ChallengeRuns_Challenges_ChallengePK",
                        column: x => x.ChallengePK,
                        principalTable: "Challenges",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChallengeRuns_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChallengeRuns_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChallengeRuns_Users_UserPK",
                        column: x => x.UserPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChaosEventInstances",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    SessionPK = table.Column<Guid>(type: "uuid", nullable: false),
                    DefinitionPK = table.Column<Guid>(type: "uuid", nullable: false),
                    EventSequence = table.Column<int>(type: "integer", nullable: false),
                    TriggeredDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RespondByDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResolvedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResolvedByUserPK = table.Column<Guid>(type: "uuid", nullable: true),
                    ActionCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResultCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    EffectSnapshot = table.Column<string>(type: "jsonb", nullable: false),
                    SourceType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SourceCode = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChaosEventInstances", x => x.PK);
                    table.CheckConstraint("CK_ChaosEventInstances_Status", "\"Status\" IN ('WARNING', 'ACTIVE', 'RESOLVED', 'FAILED', 'EXPIRED', 'CANCELLED')");
                    table.ForeignKey(
                        name: "FK_ChaosEventInstances_ChaosEventDefinitions_DefinitionPK",
                        column: x => x.DefinitionPK,
                        principalTable: "ChaosEventDefinitions",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChaosEventInstances_GameSessions_SessionPK",
                        column: x => x.SessionPK,
                        principalTable: "GameSessions",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChaosEventInstances_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChaosEventInstances_Users_ResolvedByUserPK",
                        column: x => x.ResolvedByUserPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChaosEventInstances_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    SessionPK = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderSequence = table.Column<int>(type: "integer", nullable: false),
                    OrderType = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    MenuCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RequestedTaste = table.Column<string>(type: "jsonb", nullable: false),
                    SpawnedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PatienceDeadlineDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResolvedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IsDelivery = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.PK);
                    table.CheckConstraint("CK_Orders_Status", "\"Status\" IN ('QUEUED', 'IN_PROGRESS', 'SERVED', 'CANCELLED', 'DROPPED', 'EXPIRED')");
                    table.ForeignKey(
                        name: "FK_Orders_GameSessions_SessionPK",
                        column: x => x.SessionPK,
                        principalTable: "GameSessions",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderResults",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    OrderPK = table.Column<Guid>(type: "uuid", nullable: false),
                    PreparedTaste = table.Column<string>(type: "jsonb", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    AccuracyBonus = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    DeviationPenalty = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    PaymentMultiplier = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    DeliveryMultiplier = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    FinalRevenue = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    SatisfactionScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    EarnedStars = table.Column<int>(type: "integer", nullable: false),
                    CalculationVersion = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderResults", x => x.PK);
                    table.CheckConstraint("CK_OrderResults_Status", "\"Status\" IN ('POSTED', 'VOIDED')");
                    table.ForeignKey(
                        name: "FK_OrderResults_Orders_OrderPK",
                        column: x => x.OrderPK,
                        principalTable: "Orders",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderResults_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderResults_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX "IX_CodeCounters_PeriodYYMM_TableCode"
                    ON "CodeCounters" ("PeriodYYMM", "TableCode");

                CREATE OR REPLACE FUNCTION next_public_code(p_prefix text, p_table_code char(1))
                RETURNS text
                LANGUAGE plpgsql
                AS $$
                DECLARE
                    v_period char(4) := to_char(clock_timestamp() AT TIME ZONE 'UTC', 'YYMM')::char(4);
                    v_number bigint;
                BEGIN
                    INSERT INTO "CodeCounters"
                        ("PK", "Code", "CreatedDate", "UpdatedDate", "Status", "PeriodYYMM", "TableCode", "NextNumber")
                    VALUES
                        (gen_random_uuid(), 'Sequence' || v_period || p_table_code, CURRENT_TIMESTAMP,
                         CURRENT_TIMESTAMP, 'ACTIVE', v_period, p_table_code, 2)
                    ON CONFLICT ("PeriodYYMM", "TableCode")
                    DO UPDATE SET "NextNumber" = "CodeCounters"."NextNumber" + 1,
                                  "UpdatedDate" = clock_timestamp()
                    RETURNING "NextNumber" - 1 INTO v_number;

                    IF v_number > 999999 THEN
                        RAISE EXCEPTION 'Monthly Code sequence exhausted for table code %', p_table_code;
                    END IF;

                    RETURN p_prefix || v_period || p_table_code || lpad(v_number::text, 6, '0');
                END;
                $$;

                CREATE OR REPLACE FUNCTION maintain_public_code()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    IF TG_OP = 'INSERT' THEN
                        IF NULLIF(NEW."Code", '') IS NULL THEN
                            NEW."Code" := next_public_code(TG_ARGV[0], TG_ARGV[1]::char(1));
                        END IF;
                        RETURN NEW;
                    END IF;

                    IF OLD."Code" IS NOT NULL AND OLD."Code" <> '' THEN
                        IF NEW."Code" IS DISTINCT FROM OLD."Code" THEN
                            RAISE EXCEPTION 'Code is immutable once assigned';
                        END IF;
                    ELSIF NULLIF(NEW."Code", '') IS NULL THEN
                        NEW."Code" := next_public_code(TG_ARGV[0], TG_ARGV[1]::char(1));
                    END IF;

                    NEW."CreatedDate" := OLD."CreatedDate";
                    NEW."CreatedBy" := OLD."CreatedBy";
                    NEW."UpdatedDate" := clock_timestamp();
                    RETURN NEW;
                END;
                $$;

                DO $$
                DECLARE
                    v_table record;
                BEGIN
                    FOR v_table IN
                        SELECT * FROM (VALUES
                            ('Users', 'User', 'A'),
                            ('AspNetRoles', 'Role', 'B'),
                            ('AspNetUserRoles', 'UserRole', 'C'),
                            ('RefreshTokens', 'RefreshToken', 'D'),
                            ('AuditLogEntries', 'AuditLog', 'E'),
                            ('IdempotencyRecords', 'Idempotency', 'F'),
                            ('Stages', 'Stage', 'G'),
                            ('StageConfigs', 'StageConfig', 'H'),
                            ('PlayerStageProgress', 'Progress', 'I'),
                            ('GameSessions', 'Session', 'J'),
                            ('Orders', 'Order', 'K'),
                            ('OrderResults', 'OrderResult', 'L'),
                            ('Wallets', 'Wallet', 'M'),
                            ('WalletTransactions', 'WalletTxn', 'N'),
                            ('Upgrades', 'Upgrade', 'O'),
                            ('PlayerUpgrades', 'PlayerUpgrade', 'P'),
                            ('ChaosEventDefinitions', 'ChaosDef', 'Q'),
                            ('ChaosEventInstances', 'ChaosEvent', 'R'),
                            ('Challenges', 'Challenge', 'S'),
                            ('ChallengeRuns', 'ChallengeRun', 'T'),
                            ('Matches', 'Match', 'U'),
                            ('MatchParticipants', 'Participant', 'V'),
                            ('MatchActions', 'MatchAction', 'W'),
                            ('MatchResults', 'MatchResult', 'X'),
                            ('SabotageCards', 'SabotageCard', 'Y'),
                            ('PlayerLoadouts', 'Loadout', 'Z'),
                            ('PlayerLoadoutCards', 'LoadoutCard', '0'),
                            ('MatchSabotageEvents', 'SabotageEvent', '1')
                        ) AS code_tables(table_name, code_prefix, table_code)
                    LOOP
                        EXECUTE format(
                            'CREATE TRIGGER %I BEFORE INSERT OR UPDATE ON %I FOR EACH ROW EXECUTE FUNCTION maintain_public_code(%L, %L)',
                            'trg_' || v_table.table_name || '_public_code',
                            v_table.table_name,
                            v_table.code_prefix,
                            v_table.table_code);
                    END LOOP;
                END;
                $$;

                UPDATE "Users"
                SET "Code" = next_public_code('User', 'A'),
                    "Status" = CASE WHEN "IsSuspended" THEN 'SUSPENDED' ELSE 'ACTIVE' END;

                UPDATE "AspNetRoles"
                SET "Code" = next_public_code('Role', 'B');

                UPDATE "AspNetUserRoles"
                SET "Code" = next_public_code('UserRole', 'C');

                UPDATE "RefreshTokens"
                SET "Code" = next_public_code('RefreshToken', 'D'),
                    "Status" = CASE
                        WHEN "RevokedDate" IS NOT NULL THEN 'REVOKED'
                        WHEN "ExpiresDate" <= CURRENT_TIMESTAMP THEN 'EXPIRED'
                        ELSE 'ACTIVE'
                    END;

                UPDATE "AuditLogEntries"
                SET "Code" = next_public_code('AuditLog', 'E');

                ALTER TABLE "Users" ALTER COLUMN "Code" SET NOT NULL;
                ALTER TABLE "AspNetRoles" ALTER COLUMN "Code" SET NOT NULL;
                ALTER TABLE "AspNetUserRoles" ALTER COLUMN "Code" SET NOT NULL;
                ALTER TABLE "RefreshTokens" ALTER COLUMN "Code" SET NOT NULL;
                ALTER TABLE "AuditLogEntries" ALTER COLUMN "Code" SET NOT NULL;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Code",
                table: "RefreshTokens",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_CreatedBy",
                table: "RefreshTokens",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ReplacedByPK",
                table: "RefreshTokens",
                column: "ReplacedByPK");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UpdatedBy",
                table: "RefreshTokens",
                column: "UpdatedBy");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RefreshTokens_Status",
                table: "RefreshTokens",
                sql: "\"Status\" IN ('ACTIVE', 'REVOKED', 'EXPIRED')");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_ActorUserId",
                table: "AuditLogEntries",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_Code",
                table: "AuditLogEntries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_CreatedBy",
                table: "AuditLogEntries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_UpdatedBy",
                table: "AuditLogEntries",
                column: "UpdatedBy");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AuditLogEntries_Status",
                table: "AuditLogEntries",
                sql: "\"Status\" = 'RECORDED'");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_Code",
                table: "AspNetUserRoles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_CreatedBy",
                table: "AspNetUserRoles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UpdatedBy",
                table: "AspNetUserRoles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserPK_RolePK",
                table: "AspNetUserRoles",
                columns: new[] { "UserPK", "RolePK" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_AspNetUserRoles_Status",
                table: "AspNetUserRoles",
                sql: "\"Status\" IN ('ACTIVE', 'REVOKED')");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_Code",
                table: "AspNetRoles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_CreatedBy",
                table: "AspNetRoles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_UpdatedBy",
                table: "AspNetRoles",
                column: "UpdatedBy");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AspNetRoles_Status",
                table: "AspNetRoles",
                sql: "\"Status\" IN ('ACTIVE', 'INACTIVE')");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Code",
                table: "Users",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SuspendedByPK",
                table: "Users",
                column: "SuspendedByPK");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedBy",
                table: "Users",
                column: "UpdatedBy");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Status",
                table: "Users",
                sql: "\"Status\" IN ('ACTIVE', 'SUSPENDED', 'DISABLED', 'DELETED')");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRuns_ChallengePK_Status_CalculatedScore_SubmittedD~",
                table: "ChallengeRuns",
                columns: new[] { "ChallengePK", "Status", "CalculatedScore", "SubmittedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRuns_ChallengePK_UserPK_RunSequence",
                table: "ChallengeRuns",
                columns: new[] { "ChallengePK", "UserPK", "RunSequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRuns_Code",
                table: "ChallengeRuns",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRuns_CreatedBy",
                table: "ChallengeRuns",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRuns_UpdatedBy",
                table: "ChallengeRuns",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRuns_UserPK",
                table: "ChallengeRuns",
                column: "UserPK");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ChallengeKey",
                table: "Challenges",
                column: "ChallengeKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_Code",
                table: "Challenges",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_CreatedBy",
                table: "Challenges",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_StagePK_ConfigVersion",
                table: "Challenges",
                columns: new[] { "StagePK", "ConfigVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_UpdatedBy",
                table: "Challenges",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventDefinitions_Code",
                table: "ChaosEventDefinitions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventDefinitions_CreatedBy",
                table: "ChaosEventDefinitions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventDefinitions_RulesetKey_EventKey",
                table: "ChaosEventDefinitions",
                columns: new[] { "RulesetKey", "EventKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventDefinitions_UpdatedBy",
                table: "ChaosEventDefinitions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventInstances_Code",
                table: "ChaosEventInstances",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventInstances_CreatedBy",
                table: "ChaosEventInstances",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventInstances_DefinitionPK",
                table: "ChaosEventInstances",
                column: "DefinitionPK");

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventInstances_ResolvedByUserPK",
                table: "ChaosEventInstances",
                column: "ResolvedByUserPK");

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventInstances_SessionPK_EventSequence",
                table: "ChaosEventInstances",
                columns: new[] { "SessionPK", "EventSequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChaosEventInstances_UpdatedBy",
                table: "ChaosEventInstances",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CodeCounters_Code",
                table: "CodeCounters",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodeCounters_CreatedBy",
                table: "CodeCounters",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CodeCounters_UpdatedBy",
                table: "CodeCounters",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_Code",
                table: "GameSessions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_CreatedBy",
                table: "GameSessions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_StagePK_ConfigVersion",
                table: "GameSessions",
                columns: new[] { "StagePK", "ConfigVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_UpdatedBy",
                table: "GameSessions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_UserPK_StartedDate",
                table: "GameSessions",
                columns: new[] { "UserPK", "StartedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRecords_ActorUserPK_Scope_IdempotencyKey",
                table: "IdempotencyRecords",
                columns: new[] { "ActorUserPK", "Scope", "IdempotencyKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRecords_Code",
                table: "IdempotencyRecords",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRecords_CreatedBy",
                table: "IdempotencyRecords",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRecords_UpdatedBy",
                table: "IdempotencyRecords",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_Code",
                table: "MatchActions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_CreatedBy",
                table: "MatchActions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_MatchPK_ParticipantPK_SequenceNumber",
                table: "MatchActions",
                columns: new[] { "MatchPK", "ParticipantPK", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_ParticipantPK",
                table: "MatchActions",
                column: "ParticipantPK");

            migrationBuilder.CreateIndex(
                name: "IX_MatchActions_UpdatedBy",
                table: "MatchActions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Code",
                table: "Matches",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CreatedBy",
                table: "Matches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_UpdatedBy",
                table: "Matches",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatchParticipants_Code",
                table: "MatchParticipants",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchParticipants_CreatedBy",
                table: "MatchParticipants",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatchParticipants_MatchPK_UserPK",
                table: "MatchParticipants",
                columns: new[] { "MatchPK", "UserPK" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchParticipants_UpdatedBy",
                table: "MatchParticipants",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatchParticipants_UserPK_JoinedDate",
                table: "MatchParticipants",
                columns: new[] { "UserPK", "JoinedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_Code",
                table: "MatchResults",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_CreatedBy",
                table: "MatchResults",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_MatchPK_ParticipantPK",
                table: "MatchResults",
                columns: new[] { "MatchPK", "ParticipantPK" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_ParticipantPK",
                table: "MatchResults",
                column: "ParticipantPK");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_UpdatedBy",
                table: "MatchResults",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSabotageEvents_CardPK",
                table: "MatchSabotageEvents",
                column: "CardPK");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSabotageEvents_Code",
                table: "MatchSabotageEvents",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchSabotageEvents_CreatedBy",
                table: "MatchSabotageEvents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSabotageEvents_MatchPK_EventSequence",
                table: "MatchSabotageEvents",
                columns: new[] { "MatchPK", "EventSequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchSabotageEvents_SourceParticipantPK",
                table: "MatchSabotageEvents",
                column: "SourceParticipantPK");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSabotageEvents_TargetParticipantPK",
                table: "MatchSabotageEvents",
                column: "TargetParticipantPK");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSabotageEvents_UpdatedBy",
                table: "MatchSabotageEvents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderResults_Code",
                table: "OrderResults",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderResults_CreatedBy",
                table: "OrderResults",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderResults_OrderPK",
                table: "OrderResults",
                column: "OrderPK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderResults_UpdatedBy",
                table: "OrderResults",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Code",
                table: "Orders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedBy",
                table: "Orders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SessionPK_OrderSequence",
                table: "Orders",
                columns: new[] { "SessionPK", "OrderSequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UpdatedBy",
                table: "Orders",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadoutCards_CardPK",
                table: "PlayerLoadoutCards",
                column: "CardPK");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadoutCards_Code",
                table: "PlayerLoadoutCards",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadoutCards_CreatedBy",
                table: "PlayerLoadoutCards",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadoutCards_LoadoutPK_CardPK_SlotType",
                table: "PlayerLoadoutCards",
                columns: new[] { "LoadoutPK", "CardPK", "SlotType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadoutCards_LoadoutPK_SlotType_SlotNumber",
                table: "PlayerLoadoutCards",
                columns: new[] { "LoadoutPK", "SlotType", "SlotNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadoutCards_UpdatedBy",
                table: "PlayerLoadoutCards",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadouts_Code",
                table: "PlayerLoadouts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadouts_CreatedBy",
                table: "PlayerLoadouts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadouts_UpdatedBy",
                table: "PlayerLoadouts",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLoadouts_UserPK_RulesetKey_Name",
                table: "PlayerLoadouts",
                columns: new[] { "UserPK", "RulesetKey", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStageProgress_Code",
                table: "PlayerStageProgress",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStageProgress_CreatedBy",
                table: "PlayerStageProgress",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStageProgress_StagePK",
                table: "PlayerStageProgress",
                column: "StagePK");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStageProgress_UpdatedBy",
                table: "PlayerStageProgress",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStageProgress_UserPK_StagePK",
                table: "PlayerStageProgress",
                columns: new[] { "UserPK", "StagePK" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUpgrades_Code",
                table: "PlayerUpgrades",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUpgrades_CreatedBy",
                table: "PlayerUpgrades",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUpgrades_UpdatedBy",
                table: "PlayerUpgrades",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUpgrades_UpgradePK",
                table: "PlayerUpgrades",
                column: "UpgradePK");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUpgrades_UserPK_UpgradePK",
                table: "PlayerUpgrades",
                columns: new[] { "UserPK", "UpgradePK" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SabotageCards_Code",
                table: "SabotageCards",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SabotageCards_CreatedBy",
                table: "SabotageCards",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SabotageCards_RulesetKey_CardKey",
                table: "SabotageCards",
                columns: new[] { "RulesetKey", "CardKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SabotageCards_UpdatedBy",
                table: "SabotageCards",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StageConfigs_Code",
                table: "StageConfigs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StageConfigs_CreatedBy",
                table: "StageConfigs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StageConfigs_StagePK_ConfigVersion",
                table: "StageConfigs",
                columns: new[] { "StagePK", "ConfigVersion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StageConfigs_UpdatedBy",
                table: "StageConfigs",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_Code",
                table: "Stages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stages_CreatedBy",
                table: "Stages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_StageKey",
                table: "Stages",
                column: "StageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stages_UpdatedBy",
                table: "Stages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Upgrades_Code",
                table: "Upgrades",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Upgrades_CreatedBy",
                table: "Upgrades",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Upgrades_UpdatedBy",
                table: "Upgrades",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Upgrades_UpgradeKey",
                table: "Upgrades",
                column: "UpgradeKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Code",
                table: "Wallets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_CreatedBy",
                table: "Wallets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UpdatedBy",
                table: "Wallets",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserPK_CurrencyCode",
                table: "Wallets",
                columns: new[] { "UserPK", "CurrencyCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_Code",
                table: "WalletTransactions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_CreatedBy",
                table: "WalletTransactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ReversalOfPK",
                table: "WalletTransactions",
                column: "ReversalOfPK");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_UpdatedBy",
                table: "WalletTransactions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletPK_CreatedDate",
                table: "WalletTransactions",
                columns: new[] { "WalletPK", "CreatedDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_Users_CreatedBy",
                table: "AspNetRoles",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_Users_UpdatedBy",
                table: "AspNetRoles",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_Users_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_Users_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RolePK",
                table: "AspNetUserRoles",
                column: "RolePK",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Users_CreatedBy",
                table: "AspNetUserRoles",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Users_UpdatedBy",
                table: "AspNetUserRoles",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Users_UserPK",
                table: "AspNetUserRoles",
                column: "UserPK",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_Users_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogEntries_Users_ActorUserId",
                table: "AuditLogEntries",
                column: "ActorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogEntries_Users_CreatedBy",
                table: "AuditLogEntries",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogEntries_Users_UpdatedBy",
                table: "AuditLogEntries",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_RefreshTokens_ReplacedByPK",
                table: "RefreshTokens",
                column: "ReplacedByPK",
                principalTable: "RefreshTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_CreatedBy",
                table: "RefreshTokens",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UpdatedBy",
                table: "RefreshTokens",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserPK",
                table: "RefreshTokens",
                column: "UserPK",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_SuspendedByPK",
                table: "Users",
                column: "SuspendedByPK",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UpdatedBy",
                table: "Users",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS "trg_Users_public_code" ON "Users";
                DROP TRIGGER IF EXISTS "trg_AspNetRoles_public_code" ON "AspNetRoles";
                DROP TRIGGER IF EXISTS "trg_AspNetUserRoles_public_code" ON "AspNetUserRoles";
                DROP TRIGGER IF EXISTS "trg_RefreshTokens_public_code" ON "RefreshTokens";
                DROP TRIGGER IF EXISTS "trg_AuditLogEntries_public_code" ON "AuditLogEntries";
                DROP FUNCTION IF EXISTS maintain_public_code();
                DROP FUNCTION IF EXISTS next_public_code(text, character);
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_Users_CreatedBy",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_Users_UpdatedBy",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_Users_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_Users_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RolePK",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_Users_CreatedBy",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_Users_UpdatedBy",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_Users_UserPK",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_Users_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogEntries_Users_ActorUserId",
                table: "AuditLogEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogEntries_Users_CreatedBy",
                table: "AuditLogEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogEntries_Users_UpdatedBy",
                table: "AuditLogEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_RefreshTokens_ReplacedByPK",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_CreatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UpdatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserPK",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_SuspendedByPK",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UpdatedBy",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ChallengeRuns");

            migrationBuilder.DropTable(
                name: "ChaosEventInstances");

            migrationBuilder.DropTable(
                name: "CodeCounters");

            migrationBuilder.DropTable(
                name: "IdempotencyRecords");

            migrationBuilder.DropTable(
                name: "MatchActions");

            migrationBuilder.DropTable(
                name: "MatchResults");

            migrationBuilder.DropTable(
                name: "MatchSabotageEvents");

            migrationBuilder.DropTable(
                name: "OrderResults");

            migrationBuilder.DropTable(
                name: "PlayerLoadoutCards");

            migrationBuilder.DropTable(
                name: "PlayerStageProgress");

            migrationBuilder.DropTable(
                name: "PlayerUpgrades");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "ChaosEventDefinitions");

            migrationBuilder.DropTable(
                name: "MatchParticipants");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "PlayerLoadouts");

            migrationBuilder.DropTable(
                name: "SabotageCards");

            migrationBuilder.DropTable(
                name: "Upgrades");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropTable(
                name: "StageConfigs");

            migrationBuilder.DropTable(
                name: "Stages");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Code",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_CreatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_ReplacedByPK",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UpdatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RefreshTokens_Status",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogEntries_ActorUserId",
                table: "AuditLogEntries");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogEntries_Code",
                table: "AuditLogEntries");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogEntries_CreatedBy",
                table: "AuditLogEntries");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogEntries_UpdatedBy",
                table: "AuditLogEntries");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AuditLogEntries_Status",
                table: "AuditLogEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_Code",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_CreatedBy",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UpdatedBy",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserPK_RolePK",
                table: "AspNetUserRoles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AspNetUserRoles_Status",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_Code",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_CreatedBy",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_UpdatedBy",
                table: "AspNetRoles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AspNetRoles_Status",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Code",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SuspendedByPK",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UpdatedBy",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "AuditLogEntries");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AuditLogEntries");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AuditLogEntries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AuditLogEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AuditLogEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "AuditLogEntries");

            migrationBuilder.DropColumn(
                name: "PK",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserPK",
                table: "RefreshTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "RevokedDate",
                table: "RefreshTokens",
                newName: "RevokedAtUtc");

            migrationBuilder.RenameColumn(
                name: "ReplacedByPK",
                table: "RefreshTokens",
                newName: "ReplacedByTokenId");

            migrationBuilder.RenameColumn(
                name: "ExpiresDate",
                table: "RefreshTokens",
                newName: "ExpiresAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedTokenDate",
                table: "RefreshTokens",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserPK",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameColumn(
                name: "UserPK",
                table: "AspNetUserRoles",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "RolePK",
                table: "AspNetUserRoles",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RolePK",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameColumn(
                name: "SuspendedDate",
                table: "AspNetUsers",
                newName: "SuspendedAtUtc");

            migrationBuilder.RenameColumn(
                name: "SuspendedByPK",
                table: "AspNetUsers",
                newName: "SuspendedByAdminId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "RefreshTokens",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AuditLogEntries",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
