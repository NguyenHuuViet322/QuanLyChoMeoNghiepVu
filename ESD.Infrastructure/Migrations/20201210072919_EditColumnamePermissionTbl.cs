using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class EditColumnamePermissionTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDModuleChild",
                table: "Permission");

            migrationBuilder.AddColumn<int>(
                name: "IDModule",
                table: "Permission",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDModule",
                table: "Permission");

            migrationBuilder.AddColumn<int>(
                name: "IDModuleChild",
                table: "Permission",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
