using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class UpdateTableDoc1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Autograph",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "CodeNotation",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "CodeNumber",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "ConfidenceLevel",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "DocCode",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "FileCatalog",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "FileNotation",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "IDProfileTemplate",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "InforSign",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "IssuedDate",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "Keyword",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "OrganName",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "PageAmount",
                table: "Doc");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Doc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Autograph",
                table: "Doc",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Doc",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodeNotation",
                table: "Doc",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeNumber",
                table: "Doc",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfidenceLevel",
                table: "Doc",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Doc",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocCode",
                table: "Doc",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileCatalog",
                table: "Doc",
                type: "int",
                maxLength: 4,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FileNotation",
                table: "Doc",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "Doc",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDProfileTemplate",
                table: "Doc",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "Doc",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InforSign",
                table: "Doc",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssuedDate",
                table: "Doc",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Keyword",
                table: "Doc",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Doc",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Mode",
                table: "Doc",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Doc",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganName",
                table: "Doc",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageAmount",
                table: "Doc",
                type: "int",
                maxLength: 4,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Doc",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
