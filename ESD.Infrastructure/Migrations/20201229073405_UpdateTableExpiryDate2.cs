using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class UpdateTableExpiryDate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "ExpiryDate");

            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "ExpiryDate",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "ExpiryDate");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "ExpiryDate",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
