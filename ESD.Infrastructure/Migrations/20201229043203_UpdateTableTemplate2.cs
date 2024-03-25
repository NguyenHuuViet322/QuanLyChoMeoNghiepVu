using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class UpdateTableTemplate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "TemplateParam",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfigs",
                table: "TemplateParam",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfigs",
                table: "Template",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "TemplateParam");

            migrationBuilder.DropColumn(
                name: "IsConfigs",
                table: "TemplateParam");

            migrationBuilder.DropColumn(
                name: "IsConfigs",
                table: "Template");
        }
    }
}
