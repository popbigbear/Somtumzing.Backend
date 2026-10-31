using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Somtumzing.Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AuditLogEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Severity = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Outcome = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActorUsername = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TargetType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TargetId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    RequestId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DetailsJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_ActorUsername_OccurredAtUtc",
                table: "AuditLogEntries",
                columns: new[] { "ActorUsername", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_EventType_OccurredAtUtc",
                table: "AuditLogEntries",
                columns: new[] { "EventType", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_IpAddress_OccurredAtUtc",
                table: "AuditLogEntries",
                columns: new[] { "IpAddress", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_OccurredAtUtc",
                table: "AuditLogEntries",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_Severity_OccurredAtUtc",
                table: "AuditLogEntries",
                columns: new[] { "Severity", "OccurredAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogEntries");
        }
    }
}
