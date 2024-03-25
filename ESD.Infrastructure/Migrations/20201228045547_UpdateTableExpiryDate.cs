using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class UpdateTableExpiryDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "ExpiryDate");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "ExpiryDate",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "ExpiryDate");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "ExpiryDate",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
