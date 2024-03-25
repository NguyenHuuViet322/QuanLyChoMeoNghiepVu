using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations.DasNotify
{
    public partial class initDBNotify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Notification",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Notification");
        }
    }
}
