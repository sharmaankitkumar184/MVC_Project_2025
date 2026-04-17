using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Project.Services.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSalariesModelWithValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Salary",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Salary",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 17, 16, 2, 21, 272, DateTimeKind.Local).AddTicks(1335), "$2a$11$sAU3lC7CgYeDdn2k8P7yA.0JOu2li5ghrA5alJMntcTTcs3.tbdci" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Salary");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Salary");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 27, 20, 57, 29, 555, DateTimeKind.Local).AddTicks(4168), "$2a$11$jqM9DXhy7ccs5xcmd8KN7Onngs5sud6DrWSaNxbXKbv4McGDR2g.G" });
        }
    }
}

