using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddTablePlanAgency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IDOgan",
                table: "Plan",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PlanAgency",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    IDPlan = table.Column<int>(nullable: false),
                    IDAgency = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanAgency", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanAgency");

            migrationBuilder.DropColumn(
                name: "IDOgan",
                table: "Plan");
        }
    }
}
