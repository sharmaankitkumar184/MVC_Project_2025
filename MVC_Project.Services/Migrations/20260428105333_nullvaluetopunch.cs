using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Project.Services.Migrations
{
    /// <inheritdoc />
    public partial class nullvaluetopunch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LocationCheckOut",
                table: "PunchSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "LocationCheckIn",
                table: "PunchSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceCheckOut",
                table: "PunchSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceCheckIn",
                table: "PunchSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BreakReason",
                table: "PunchSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 28, 16, 23, 31, 582, DateTimeKind.Local).AddTicks(255), "$2a$11$nEk7oLHbY/9kPcorTQx/vOKro8xFNgH1YPuMI28lzktuu.YW9KVeG", new DateTime(2026, 4, 28, 10, 53, 31, 435, DateTimeKind.Utc).AddTicks(6562) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LocationCheckOut",
                table: "PunchSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LocationCheckIn",
                table: "PunchSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceCheckOut",
                table: "PunchSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceCheckIn",
                table: "PunchSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BreakReason",
                table: "PunchSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 27, 21, 36, 55, 423, DateTimeKind.Local).AddTicks(8976), "$2a$11$BmqXjqos8/mhg1m3YEbc0edD8e9h5mzt5jhGrG60sL4ZQgmD.r5o6", new DateTime(2026, 4, 27, 16, 6, 55, 257, DateTimeKind.Utc).AddTicks(3641) });
        }
    }
}
