using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class ProfileTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PaperTotal",
                table: "ProfileTemplate",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<long>(
                name: "PaperDigital",
                table: "ProfileTemplate",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<long>(
                name: "CopyNumber",
                table: "ProfileTemplate",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PaperTotal",
                table: "ProfileTemplate",
                type: "int",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(long),
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "PaperDigital",
                table: "ProfileTemplate",
                type: "int",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(long),
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "CopyNumber",
                table: "ProfileTemplate",
                type: "int",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(long),
                oldMaxLength: 10);
        }
    }
}
