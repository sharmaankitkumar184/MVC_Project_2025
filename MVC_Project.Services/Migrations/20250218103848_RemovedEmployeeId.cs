using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Project.Services.Migrations
{
    /// <inheritdoc />
    public partial class RemovedEmployeeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salary_Employees_EmployeeId_EmployeeCode",
                table: "Salary");

            migrationBuilder.DropIndex(
                name: "IX_Salary_EmployeeId_EmployeeCode",
                table: "Salary");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Salary");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeCode",
                table: "Salary",
                type: "nvarchar(10)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Salary_EmployeeCode",
                table: "Salary",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Salary_Employees_EmployeeCode",
                table: "Salary",
                column: "EmployeeCode",
                principalTable: "Employees",
                principalColumn: "EmployeeCode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salary_Employees_EmployeeCode",
                table: "Salary");

            migrationBuilder.DropIndex(
                name: "IX_Salary_EmployeeCode",
                table: "Salary");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Employees_EmployeeCode",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeCode",
                table: "Salary",
                type: "nvarchar(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Salary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Salary_EmployeeId_EmployeeCode",
                table: "Salary",
                columns: new[] { "EmployeeId", "EmployeeCode" },
                unique: true,
                filter: "[EmployeeCode] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Salary_Employees_EmployeeId_EmployeeCode",
                table: "Salary",
                columns: new[] { "EmployeeId", "EmployeeCode" },
                principalTable: "Employees",
                principalColumns: new[] { "Id", "EmployeeCode" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
