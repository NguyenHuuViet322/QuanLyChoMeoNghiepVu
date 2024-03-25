using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class UpdateTableTemplate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfigs",
                table: "TemplateParam");

            migrationBuilder.DropColumn(
                name: "IsConfigs",
                table: "Template");

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "TemplateParam",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "Template",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "IDOldFile",
                table: "StgFile",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDSigner",
                table: "StgFile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSign",
                table: "StgFile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SignType",
                table: "StgFile",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "TemplateParam");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "Template");

            migrationBuilder.DropColumn(
                name: "IDOldFile",
                table: "StgFile");

            migrationBuilder.DropColumn(
                name: "IDSigner",
                table: "StgFile");

            migrationBuilder.DropColumn(
                name: "IsSign",
                table: "StgFile");

            migrationBuilder.DropColumn(
                name: "SignType",
                table: "StgFile");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfigs",
                table: "TemplateParam",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfigs",
                table: "Template",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
