using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class UpdateTableOrganAgency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropColumn(
                name: "IDDepartment",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IDAgency",
                table: "Storage");

            migrationBuilder.DropColumn(
                name: "IDAgency",
                table: "ProfileTemplate");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "IsArchive",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Agency");

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "Storage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "ProfileTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FondCode",
                table: "ProfileTemplate",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "Agency",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Agency",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDOrgan",
                table: "Agency",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Organ",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    Address = table.Column<string>(maxLength: 250, nullable: true),
                    Fax = table.Column<string>(maxLength: 20, nullable: true),
                    IsArchive = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organ", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Organ");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "Storage");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "ProfileTemplate");

            migrationBuilder.DropColumn(
                name: "FondCode",
                table: "ProfileTemplate");

            migrationBuilder.DropColumn(
                name: "IDOrgan",
                table: "Agency");

            migrationBuilder.AddColumn<int>(
                name: "IDDepartment",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "Storage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Agencyld",
                table: "ProfileTemplate",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDAgency",
                table: "ProfileTemplate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AgencyName",
                table: "Doc",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "Agency",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Agency",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Agency",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Agency",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "Agency",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchive",
                table: "Agency",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Agency",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IDAgency = table.Column<int>(type: "int", nullable: false),
                    IDChannel = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.ID);
                });
        }
    }
}
