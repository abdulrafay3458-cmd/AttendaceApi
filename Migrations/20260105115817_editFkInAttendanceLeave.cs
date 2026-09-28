using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class editFkInAttendanceLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceLeave_AttendanceRecord_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year",
                schema: "App",
                table: "AttendanceLeave");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceLeave_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year",
                schema: "App",
                table: "AttendanceLeave");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceLeave_AttendanceRecord_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_year_att_leave_fortnight",
                schema: "App",
                table: "AttendanceLeave",
                columns: new[] { "att_leave_employee_code", "att_leave_day", "att_leave_month", "att_leave_year", "att_leave_fortnight" },
                principalSchema: "App",
                principalTable: "AttendanceRecord",
                principalColumns: new[] { "att_rec_employee_code", "att_rec_day", "att_rec_month", "att_rec_year", "att_rec_fortnight" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceLeave_AttendanceRecord_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_year_att_leave_fortnight",
                schema: "App",
                table: "AttendanceLeave");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLeave_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year",
                schema: "App",
                table: "AttendanceLeave",
                columns: new[] { "att_leave_employee_code", "att_leave_day", "att_leave_month", "att_leave_fortnight", "att_leave_year" });

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceLeave_AttendanceRecord_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year",
                schema: "App",
                table: "AttendanceLeave",
                columns: new[] { "att_leave_employee_code", "att_leave_day", "att_leave_month", "att_leave_fortnight", "att_leave_year" },
                principalSchema: "App",
                principalTable: "AttendanceRecord",
                principalColumns: new[] { "att_rec_employee_code", "att_rec_day", "att_rec_month", "att_rec_year", "att_rec_fortnight" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
