using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class AddTableDownloadLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DownloadLink",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    IDChannel = table.Column<int>(nullable: false),
                    DownloadHash = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ExpiredAt = table.Column<DateTime>(nullable: true),
                    IDFile = table.Column<long>(nullable: true),
                    IDFolder = table.Column<int>(nullable: true),
                    CreatedBy = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownloadLink", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DownloadLink");
        }
    }
}
