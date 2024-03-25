using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddTablePlanProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDAgency",
                table: "Profile");

            migrationBuilder.CreateTable(
                name: "PlanProfile",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDPlan = table.Column<int>(nullable: false),
                    FileCode = table.Column<string>(maxLength: 30, nullable: true),
                    IDStorage = table.Column<int>(nullable: false),
                    IDCodeBox = table.Column<int>(nullable: false),
                    IDProfileList = table.Column<int>(nullable: false),
                    IDSecurityLevel = table.Column<int>(nullable: false),
                    IDProfileTemplate = table.Column<int>(nullable: false),
                    Identifier = table.Column<string>(maxLength: 13, nullable: true),
                    FileCatalog = table.Column<int>(maxLength: 4, nullable: false),
                    FileNotation = table.Column<string>(maxLength: 20, nullable: true),
                    Title = table.Column<string>(maxLength: 1000, nullable: true),
                    IDExpiryDate = table.Column<int>(nullable: false),
                    Rights = table.Column<string>(maxLength: 30, nullable: true),
                    Language = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    TotalDoc = table.Column<int>(maxLength: 10, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    InforSign = table.Column<string>(maxLength: 30, nullable: true),
                    Keyword = table.Column<string>(maxLength: 100, nullable: true),
                    Maintenance = table.Column<int>(maxLength: 10, nullable: false),
                    PageNumber = table.Column<int>(maxLength: 10, nullable: false),
                    Format = table.Column<string>(maxLength: 20, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IDAgency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanProfile", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanProfile");

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "Profile",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
