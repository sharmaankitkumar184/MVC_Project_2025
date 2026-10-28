using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Project.Services.Migrations
{
    /// <inheritdoc />
    public partial class addpunchsessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PunchSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendanceId = table.Column<int>(type: "int", nullable: false),
                    SessionNumber = table.Column<int>(type: "int", nullable: false),
                    PunchInTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LocationCheckIn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocationCheckOut = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeviceCheckIn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeviceCheckOut = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BreakReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunchSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PunchSessions_Attendances_AttendanceId",
                        column: x => x.AttendanceId,
                        principalTable: "Attendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 27, 21, 36, 55, 423, DateTimeKind.Local).AddTicks(8976), "$2a$11$BmqXjqos8/mhg1m3YEbc0edD8e9h5mzt5jhGrG60sL4ZQgmD.r5o6", new DateTime(2026, 4, 27, 16, 6, 55, 257, DateTimeKind.Utc).AddTicks(3641) });

            migrationBuilder.CreateIndex(
                name: "IX_PunchSessions_AttendanceId_SessionNumber",
                table: "PunchSessions",
                columns: new[] { "AttendanceId", "SessionNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PunchSessions");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 27, 16, 24, 33, 465, DateTimeKind.Local).AddTicks(9152), "$2a$11$GmbBZkUegZHzGuEP6t0ehe.q.0hE8oUrqJj9m30ig9eZT29MIOppq", new DateTime(2026, 4, 27, 10, 54, 33, 334, DateTimeKind.Utc).AddTicks(7956) });
        }
    }
}
