using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class UpdateIDAgency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "ProfileTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "ProfileList",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "CategoryTypeField",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "CategoryType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "CategoryField",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "Category",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDAgency",
                table: "ProfileTemplate");

            migrationBuilder.DropColumn(
                name: "IDAgency",
                table: "ProfileList");

            migrationBuilder.DropColumn(
                name: "IDAgency",
                table: "CategoryTypeField");

            migrationBuilder.DropColumn(
                name: "IDAgency",
                table: "CategoryType");

            migrationBuilder.DropColumn(
                name: "IDAgency",
                table: "CategoryField");

            migrationBuilder.DropColumn(
                name: "IDAgency",
                table: "Category");
        }
    }
}
