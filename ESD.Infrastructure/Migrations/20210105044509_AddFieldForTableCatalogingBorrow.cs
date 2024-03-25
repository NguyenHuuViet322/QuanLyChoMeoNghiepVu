using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddFieldForTableCatalogingBorrow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOriginal",
                table: "CatalogingBorrow",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "CatalogingBorrow",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "CatalogingBorrow",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOriginal",
                table: "CatalogingBorrow");

            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "CatalogingBorrow");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "CatalogingBorrow");
        }
    }
}
