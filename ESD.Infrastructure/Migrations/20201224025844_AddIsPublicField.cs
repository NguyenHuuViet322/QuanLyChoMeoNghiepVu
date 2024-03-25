using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddIsPublicField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "CatalogingProfile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "CatalogingDoc",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "CatalogingProfile");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "CatalogingDoc");
        }
    }
}
