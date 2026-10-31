using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Somtumzing.Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class PlayerSuspension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SuspendedAtUtc",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SuspendedByAdminId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspensionReason",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SuspendedAtUtc",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SuspendedByAdminId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SuspensionReason",
                table: "AspNetUsers");
        }
    }
}
