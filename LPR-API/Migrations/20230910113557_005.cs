using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPR_API.Migrations
{
    /// <inheritdoc />
    public partial class _005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "key_description",
                table: "ApiKeys",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "key_description",
                table: "ApiKeys");
        }
    }
}
