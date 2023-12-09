using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPR_API.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class _010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FileUploaded_uploaded_api",
                table: "FileUploaded",
                column: "uploaded_api");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploaded_vehicle_brand",
                table: "FileUploaded",
                column: "vehicle_brand");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploaded_vehicle_class",
                table: "FileUploaded",
                column: "vehicle_class");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploaded_vehicle_license",
                table: "FileUploaded",
                column: "vehicle_license");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploaded_vehicle_province",
                table: "FileUploaded",
                column: "vehicle_province");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileUploaded_uploaded_api",
                table: "FileUploaded");

            migrationBuilder.DropIndex(
                name: "IX_FileUploaded_vehicle_brand",
                table: "FileUploaded");

            migrationBuilder.DropIndex(
                name: "IX_FileUploaded_vehicle_class",
                table: "FileUploaded");

            migrationBuilder.DropIndex(
                name: "IX_FileUploaded_vehicle_license",
                table: "FileUploaded");

            migrationBuilder.DropIndex(
                name: "IX_FileUploaded_vehicle_province",
                table: "FileUploaded");
        }
    }
}
