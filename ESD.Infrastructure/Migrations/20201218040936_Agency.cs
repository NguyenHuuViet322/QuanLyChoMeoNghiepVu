using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class Agency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentPath",
                table: "Agency",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentPath",
                table: "Agency");
        }
    }
}
