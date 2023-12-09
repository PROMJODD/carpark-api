using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPR_API.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class _008 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileUploaded",
                columns: table => new
                {
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    org_id = table.Column<string>(type: "text", nullable: true),
                    identity_type = table.Column<string>(type: "text", nullable: true),
                    uploader_id = table.Column<Guid>(type: "uuid", nullable: false),
                    storage_path = table.Column<string>(type: "text", nullable: true),
                    recognition_status = table.Column<string>(type: "text", nullable: true),
                    recognition_message = table.Column<string>(type: "text", nullable: true),
                    vehicle_license = table.Column<string>(type: "text", nullable: true),
                    vehicle_province = table.Column<string>(type: "text", nullable: true),
                    vehicle_brand = table.Column<string>(type: "text", nullable: true),
                    vehicle_class = table.Column<string>(type: "text", nullable: true),
                    vehicle_color = table.Column<string>(type: "text", nullable: true),
                    quota_left = table.Column<long>(type: "bigint", nullable: true),
                    uploaded_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    uploaded_api = table.Column<string>(type: "text", nullable: true),
                    file_size = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUploaded", x => x.file_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileUploaded_storage_path",
                table: "FileUploaded",
                column: "storage_path",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileUploaded");
        }
    }
}
