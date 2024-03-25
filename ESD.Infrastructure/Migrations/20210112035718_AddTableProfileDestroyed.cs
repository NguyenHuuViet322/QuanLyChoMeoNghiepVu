using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddTableProfileDestroyed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfileDestroyed",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDDestruction = table.Column<int>(nullable: false),
                    IDCatalogingProfile = table.Column<int>(nullable: false),
                    IDOrgan = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 1000, nullable: true),
                    FileCode = table.Column<string>(maxLength: 30, nullable: true),
                    IDExpiryDate = table.Column<int>(nullable: false),
                    IDStorage = table.Column<int>(nullable: false),
                    IDShelve = table.Column<int>(nullable: false),
                    IDBox = table.Column<int>(nullable: false),
                    InUsing = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileDestroyed", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileDestroyed");
        }
    }
}
