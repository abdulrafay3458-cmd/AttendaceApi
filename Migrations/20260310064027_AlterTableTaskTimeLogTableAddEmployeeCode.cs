using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableTaskTimeLogTableAddEmployeeCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "time_log_employee_code",
                schema: "App",
                table: "TaskTimeLogs",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTimeLogs_time_log_employee_code",
                schema: "App",
                table: "TaskTimeLogs",
                column: "time_log_employee_code");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Employee_time_log_employee_code",
                schema: "App",
                table: "TaskTimeLogs",
                column: "time_log_employee_code",
                principalSchema: "General",
                principalTable: "Employee",
                principalColumn: "employee_code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Employee_time_log_employee_code",
                schema: "App",
                table: "TaskTimeLogs");

            migrationBuilder.DropIndex(
                name: "IX_TaskTimeLogs_time_log_employee_code",
                schema: "App",
                table: "TaskTimeLogs");

            migrationBuilder.DropColumn(
                name: "time_log_employee_code",
                schema: "App",
                table: "TaskTimeLogs");
        }
    }
}
