using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPR_API.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class _011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationsUsers",
                columns: table => new
                {
                    org_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    org_custom_id = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    user_name = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    roles_list = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationsUsers", x => x.org_user_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: true),
                    user_email = table.Column<string>(type: "text", nullable: true),
                    user_created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationsUsers_org_custom_id",
                table: "OrganizationsUsers",
                column: "org_custom_id");

            migrationBuilder.CreateIndex(
                name: "OrgUser_Unique1",
                table: "OrganizationsUsers",
                columns: new[] { "org_custom_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_user_email",
                table: "Users",
                column: "user_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_user_name",
                table: "Users",
                column: "user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationsUsers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
