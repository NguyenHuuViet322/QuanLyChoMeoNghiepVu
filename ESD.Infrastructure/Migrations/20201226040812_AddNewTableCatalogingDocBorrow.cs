using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddNewTableCatalogingDocBorrow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogingDocBorrow",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDFile = table.Column<long>(nullable: false),
                    IDProfile = table.Column<int>(nullable: false),
                    IDDoc = table.Column<int>(nullable: false),
                    IDDocType = table.Column<int>(nullable: false),
                    IDReader = table.Column<int>(nullable: false),
                    ApproveBy = table.Column<int>(nullable: true),
                    ApproveDate = table.Column<DateTime>(nullable: true),
                    FromDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogingDocBorrow", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogingDocBorrow");
        }
    }
}
