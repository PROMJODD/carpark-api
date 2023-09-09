using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPR_API.Migrations
{
    /// <inheritdoc />
    public partial class _003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "org_custom_id",
                table: "Organizations",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_org_custom_id",
                table: "Organizations",
                column: "org_custom_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizations_org_custom_id",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "org_custom_id",
                table: "Organizations");
        }
    }
}
