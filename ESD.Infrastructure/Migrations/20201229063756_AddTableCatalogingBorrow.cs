using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddTableCatalogingBorrow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogingDocBorrow");

            migrationBuilder.CreateTable(
                name: "CatalogingBorrow",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDOrgan = table.Column<int>(nullable: false),
                    IDReader = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Purpose = table.Column<string>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    ApproveBy = table.Column<int>(nullable: true),
                    ApproveDate = table.Column<DateTime>(nullable: true),
                    ReasonToReject = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogingBorrow", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CatalogingBorrowDoc",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDCatalogingBorrow = table.Column<int>(nullable: false),
                    IDProfile = table.Column<int>(nullable: false),
                    IDDoc = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogingBorrowDoc", x => x.ID);
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogingBorrow");

            migrationBuilder.DropTable(
                name: "CatalogingBorrowDoc");


            migrationBuilder.CreateTable(
                name: "CatalogingDocBorrow",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApproveBy = table.Column<int>(type: "int", nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IDChannel = table.Column<int>(type: "int", nullable: false),
                    IDDoc = table.Column<int>(type: "int", nullable: false),
                    IDDocType = table.Column<int>(type: "int", nullable: false),
                    IDFile = table.Column<long>(type: "bigint", nullable: false),
                    IDProfile = table.Column<int>(type: "int", nullable: false),
                    IDReader = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogingDocBorrow", x => x.ID);
                });
        }
    }
}
