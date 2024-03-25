using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddIDOganForProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IDOgan",
                table: "Profile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOgan",
                table: "PlanProfile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOgan",
                table: "CatalogingProfile",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
