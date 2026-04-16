using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Project.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Activities",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Colleagues",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 27, 20, 57, 29, 555, DateTimeKind.Local).AddTicks(4168), "$2a$11$jqM9DXhy7ccs5xcmd8KN7Onngs5sud6DrWSaNxbXKbv4McGDR2g.G" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activities",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Colleagues",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 6, 13, 38, 13, 954, DateTimeKind.Local).AddTicks(8285), "$2a$11$J58OBSgtIM2Xds6KKiXm/.yIeBeKxfbBVpXIl0D5TM/bPqn34EoRW" });
        }
    }
}
