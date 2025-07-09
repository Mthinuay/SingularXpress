using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SingularExpress.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadedFieldsToTaxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UploadedByUserId",
                table: "TaxTables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedDate",
                table: "TaxTables",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "TaxTables");

            migrationBuilder.DropColumn(
                name: "UploadedDate",
                table: "TaxTables");
        }
    }
}
