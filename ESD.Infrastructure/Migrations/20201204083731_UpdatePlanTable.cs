using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class UpdatePlanTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReasonToReject",
                table: "Profile",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Plan",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReasonToReject",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Plan");
        }
    }
}
