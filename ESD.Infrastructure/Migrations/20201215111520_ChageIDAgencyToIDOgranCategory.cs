using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class ChageIDAgencyToIDOgranCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "CategoryTypeField",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "CategoryType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "CategoryField",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "Category",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "CategoryTypeField");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "CategoryType");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "CategoryField");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "Category");

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "CategoryTypeField",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "CategoryType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "CategoryField",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
