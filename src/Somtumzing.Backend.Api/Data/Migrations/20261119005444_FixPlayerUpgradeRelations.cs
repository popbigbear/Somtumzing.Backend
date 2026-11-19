using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Somtumzing.Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPlayerUpgradeRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_PlayerUpgrades_Users_UserPK",
            //    table: "PlayerUpgrades");

            //migrationBuilder.RenameColumn(
            //    name: "UserPK",
            //    table: "PlayerUpgrades",
            //    newName: "PlayerPK");

            //migrationBuilder.RenameIndex(
            //    name: "IX_PlayerUpgrades_UserPK_UpgradePK",
            //    table: "PlayerUpgrades",
            //    newName: "IX_PlayerUpgrades_PlayerPK_UpgradePK");

            migrationBuilder.AddColumn<int>(
                name: "BestScore",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Coins",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Energy",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalServed",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Upgrades",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    AchievementKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TargetValue = table.Column<int>(type: "integer", nullable: false),
                    RewardCoins = table.Column<int>(type: "integer", nullable: false),
                    RewardDiamonds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.PK);
                    table.CheckConstraint("CK_Achievements_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_Achievements_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Achievements_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventLeaderboards",
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
                    PlayerPK = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLeaderboards", x => x.PK);
                    table.CheckConstraint("CK_EventLeaderboards_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_EventLeaderboards_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventLeaderboards_Users_PlayerPK",
                        column: x => x.PlayerPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventLeaderboards_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameEvents",
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
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartsAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndsAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RewardConfig = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameEvents", x => x.PK);
                    table.CheckConstraint("CK_GameEvents_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_GameEvents_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEvents_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    MissionKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RewardCoins = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.PK);
                    table.CheckConstraint("CK_Missions_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_Missions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Missions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAchievements",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    PlayerPK = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsClaimed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAchievements", x => x.PK);
                    table.CheckConstraint("CK_PlayerAchievements_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_PlayerAchievements_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerAchievements_Users_PlayerPK",
                        column: x => x.PlayerPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerAchievements_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCosmetics",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    PlayerPK = table.Column<Guid>(type: "uuid", nullable: false),
                    CosmeticKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Unlocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsEquipped = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCosmetics", x => x.PK);
                    table.CheckConstraint("CK_PlayerCosmetics_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_PlayerCosmetics_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerCosmetics_Users_PlayerPK",
                        column: x => x.PlayerPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerCosmetics_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDailyRewards",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    PlayerPK = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStreak = table.Column<int>(type: "integer", nullable: false),
                    LastClaimedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TodayClaimed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDailyRewards", x => x.PK);
                    table.CheckConstraint("CK_PlayerDailyRewards_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_PlayerDailyRewards_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerDailyRewards_Users_PlayerPK",
                        column: x => x.PlayerPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerDailyRewards_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGameEvents",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    PlayerPK = table.Column<Guid>(type: "uuid", nullable: false),
                    EventKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    TargetValue = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsClaimed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGameEvents", x => x.PK);
                    table.CheckConstraint("CK_PlayerGameEvents_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_PlayerGameEvents_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerGameEvents_Users_PlayerPK",
                        column: x => x.PlayerPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerGameEvents_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerIngredients",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    PlayerPK = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerIngredients", x => x.PK);
                    table.CheckConstraint("CK_PlayerIngredients_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_PlayerIngredients_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerIngredients_Users_PlayerPK",
                        column: x => x.PlayerPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerIngredients_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerMissions",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    PlayerPK = table.Column<Guid>(type: "uuid", nullable: false),
                    MissionKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    TargetValue = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsClaimed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMissions", x => x.PK);
                    table.CheckConstraint("CK_PlayerMissions_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_PlayerMissions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerMissions_Users_PlayerPK",
                        column: x => x.PlayerPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerMissions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSettings",
                columns: table => new
                {
                    PK = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    PlayerPK = table.Column<Guid>(type: "uuid", nullable: false),
                    Sound = table.Column<bool>(type: "boolean", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Analytics = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSettings", x => x.PK);
                    table.CheckConstraint("CK_PlayerSettings_Status", "\"Status\" IN ('ACTIVE')");
                    table.ForeignKey(
                        name: "FK_PlayerSettings_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerSettings_Users_PlayerPK",
                        column: x => x.PlayerPK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerSettings_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_AchievementKey",
                table: "Achievements",
                column: "AchievementKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_Code",
                table: "Achievements",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_CreatedBy",
                table: "Achievements",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_UpdatedBy",
                table: "Achievements",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EventLeaderboards_Code",
                table: "EventLeaderboards",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventLeaderboards_CreatedBy",
                table: "EventLeaderboards",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EventLeaderboards_EventKey_PlayerPK",
                table: "EventLeaderboards",
                columns: new[] { "EventKey", "PlayerPK" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventLeaderboards_PlayerPK",
                table: "EventLeaderboards",
                column: "PlayerPK");

            migrationBuilder.CreateIndex(
                name: "IX_EventLeaderboards_UpdatedBy",
                table: "EventLeaderboards",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_Code",
                table: "GameEvents",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_CreatedBy",
                table: "GameEvents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_EventKey",
                table: "GameEvents",
                column: "EventKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_UpdatedBy",
                table: "GameEvents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_Code",
                table: "Missions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Missions_CreatedBy",
                table: "Missions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_MissionKey",
                table: "Missions",
                column: "MissionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Missions_UpdatedBy",
                table: "Missions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_Code",
                table: "PlayerAchievements",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_CreatedBy",
                table: "PlayerAchievements",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_PlayerPK_AchievementKey",
                table: "PlayerAchievements",
                columns: new[] { "PlayerPK", "AchievementKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_UpdatedBy",
                table: "PlayerAchievements",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCosmetics_Code",
                table: "PlayerCosmetics",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCosmetics_CreatedBy",
                table: "PlayerCosmetics",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCosmetics_PlayerPK_CosmeticKey",
                table: "PlayerCosmetics",
                columns: new[] { "PlayerPK", "CosmeticKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCosmetics_UpdatedBy",
                table: "PlayerCosmetics",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDailyRewards_Code",
                table: "PlayerDailyRewards",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDailyRewards_CreatedBy",
                table: "PlayerDailyRewards",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDailyRewards_PlayerPK",
                table: "PlayerDailyRewards",
                column: "PlayerPK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDailyRewards_UpdatedBy",
                table: "PlayerDailyRewards",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameEvents_Code",
                table: "PlayerGameEvents",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameEvents_CreatedBy",
                table: "PlayerGameEvents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameEvents_PlayerPK_EventKey",
                table: "PlayerGameEvents",
                columns: new[] { "PlayerPK", "EventKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameEvents_UpdatedBy",
                table: "PlayerGameEvents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerIngredients_Code",
                table: "PlayerIngredients",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerIngredients_CreatedBy",
                table: "PlayerIngredients",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerIngredients_PlayerPK_ItemKey",
                table: "PlayerIngredients",
                columns: new[] { "PlayerPK", "ItemKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerIngredients_UpdatedBy",
                table: "PlayerIngredients",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMissions_Code",
                table: "PlayerMissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMissions_CreatedBy",
                table: "PlayerMissions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMissions_PlayerPK_MissionKey",
                table: "PlayerMissions",
                columns: new[] { "PlayerPK", "MissionKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMissions_UpdatedBy",
                table: "PlayerMissions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSettings_Code",
                table: "PlayerSettings",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSettings_CreatedBy",
                table: "PlayerSettings",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSettings_PlayerPK",
                table: "PlayerSettings",
                column: "PlayerPK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSettings_UpdatedBy",
                table: "PlayerSettings",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerUpgrades_Users_PlayerPK",
                table: "PlayerUpgrades",
                column: "PlayerPK",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           // migrationBuilder.DropForeignKey(
           //     name: "FK_PlayerUpgrades_Users_PlayerPK",
           //     table: "PlayerUpgrades");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "EventLeaderboards");

            migrationBuilder.DropTable(
                name: "GameEvents");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "PlayerAchievements");

            migrationBuilder.DropTable(
                name: "PlayerCosmetics");

            migrationBuilder.DropTable(
                name: "PlayerDailyRewards");

            migrationBuilder.DropTable(
                name: "PlayerGameEvents");

            migrationBuilder.DropTable(
                name: "PlayerIngredients");

            migrationBuilder.DropTable(
                name: "PlayerMissions");

            migrationBuilder.DropTable(
                name: "PlayerSettings");

            migrationBuilder.DropColumn(
                name: "BestScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Coins",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Energy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalServed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Upgrades");

            /*migrationBuilder.RenameColumn(
                name: "PlayerPK",
                table: "PlayerUpgrades",
                newName: "UserPK");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerUpgrades_PlayerPK_UpgradePK",
                table: "PlayerUpgrades",
                newName: "IX_PlayerUpgrades_UserPK_UpgradePK");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerUpgrades_Users_UserPK",
                table: "PlayerUpgrades",
                column: "UserPK",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);*/
        }
    }
}
