using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Project.Services.Migrations
{
    /// <inheritdoc />
    public partial class fixissuewithcreateddate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 22, 12, 11, 53, 953, DateTimeKind.Local).AddTicks(3656), "$2a$11$mQ7Zzx7vYk7jtDEQqGEz/ukRdyMejaS9Ig2tudR.jMx9tN.mWY3Lq", new DateTime(2026, 4, 22, 6, 41, 53, 735, DateTimeKind.Utc).AddTicks(8840) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 22, 11, 59, 40, 208, DateTimeKind.Local).AddTicks(5681), "$2a$11$kNd2Nhk121.s8Lh0drs1w.LG2RMQbiABn4jNavz4k976WXqWh7v8a" });
        }
    }
}
