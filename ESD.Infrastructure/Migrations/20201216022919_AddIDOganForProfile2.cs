using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddIDOganForProfile2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDOgan",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "IDOgan",
                table: "PlanProfile");

            migrationBuilder.DropColumn(
                name: "IDOgan",
                table: "CatalogingProfile");

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "Profile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "PlanProfile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "CatalogingProfile",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "PlanProfile");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "CatalogingProfile");

            migrationBuilder.AddColumn<int>(
                name: "IDOgan",
                table: "Profile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOgan",
                table: "PlanProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOgan",
                table: "CatalogingProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
