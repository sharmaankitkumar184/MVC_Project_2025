using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Project.Services.Migrations
{
    /// <inheritdoc />
    public partial class attendancefeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Projects",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchInTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    PunchOutTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LeaveType = table.Column<int>(type: "int", nullable: true),
                    LeaveReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LocationCheckIn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocationCheckOut = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeviceCheckIn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeviceCheckOut = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    ApprovalRemark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 27, 15, 59, 20, 582, DateTimeKind.Local).AddTicks(7873), "$2a$11$z5TG9gra57SeYPeHZ4i0BurfV3KLph2Ed2x7sDdaUijl2v.rArj66", new DateTime(2026, 4, 27, 10, 29, 20, 189, DateTimeKind.Utc).AddTicks(5928) });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmployeeId_AttendanceDate",
                table: "Attendances",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "DateOfRegister", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 22, 12, 11, 53, 953, DateTimeKind.Local).AddTicks(3656), "$2a$11$mQ7Zzx7vYk7jtDEQqGEz/ukRdyMejaS9Ig2tudR.jMx9tN.mWY3Lq", new DateTime(2026, 4, 22, 6, 41, 53, 735, DateTimeKind.Utc).AddTicks(8840) });
        }
    }
}
