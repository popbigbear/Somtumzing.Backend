using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Somtumzing.Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUserRoleCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserPK_RolePK",
                table: "AspNetUserRoles");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AspNetUserRoles_PK",
                table: "AspNetUserRoles",
                column: "PK");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserPK", "RolePK" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_AspNetUserRoles_PK",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                column: "PK");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserPK_RolePK",
                table: "AspNetUserRoles",
                columns: new[] { "UserPK", "RolePK" },
                unique: true);
        }
    }
}
