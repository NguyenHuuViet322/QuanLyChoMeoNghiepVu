using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class UpdateDeliveryRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "DeliveryRecord",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentReceiveStatus",
                table: "DeliveryRecord",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DocumentTime",
                table: "DeliveryRecord",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordCreateDate",
                table: "DeliveryRecord",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "DeliveryRecord");

            migrationBuilder.DropColumn(
                name: "DocumentReceiveStatus",
                table: "DeliveryRecord");

            migrationBuilder.DropColumn(
                name: "DocumentTime",
                table: "DeliveryRecord");

            migrationBuilder.DropColumn(
                name: "RecordCreateDate",
                table: "DeliveryRecord");
        }
    }
}
