using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddIDProfileCategoryForProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IDProfileCategory",
                table: "Profile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDProfileCategory",
                table: "PlanProfile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDProfileCategory",
                table: "CatalogingProfile",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDProfileCategory",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "IDProfileCategory",
                table: "PlanProfile");

            migrationBuilder.DropColumn(
                name: "IDProfileCategory",
                table: "CatalogingProfile");
        }
    }
}
