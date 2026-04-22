using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Project.Services.Migrations
{
    /// <inheritdoc />
    public partial class addedfewcolumnstouser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 22, 11, 59, 40, 208, DateTimeKind.Local).AddTicks(5681), "$2a$11$kNd2Nhk121.s8Lh0drs1w.LG2RMQbiABn4jNavz4k976WXqWh7v8a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 22, 11, 34, 15, 297, DateTimeKind.Local).AddTicks(6540), "$2a$11$1iBjawIupryMV/PgRhx0VuGTM0geh7Dq2NiVmXWh.OSvJRd4o/HlO" });
        }
    }
}
