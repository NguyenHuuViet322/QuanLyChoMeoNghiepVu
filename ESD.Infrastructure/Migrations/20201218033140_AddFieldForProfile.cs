using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddFieldForProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IDBox",
                table: "CatalogingProfile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDShelve",
                table: "CatalogingProfile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsStoraged",
                table: "CatalogingProfile",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDBox",
                table: "CatalogingProfile");

            migrationBuilder.DropColumn(
                name: "IDShelve",
                table: "CatalogingProfile");

            migrationBuilder.DropColumn(
                name: "IsStoraged",
                table: "CatalogingProfile");
        }
    }
}
