using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add_Table_CancelLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CancelLeave",
                schema: "App",
                columns: table => new
                {
                    cancel_leave_Id = table.Column<Guid>(type: "uniqueidentifier", unicode: false, maxLength: 50, nullable: false),
                    cancel_leave_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    cancel_leave_year = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    cancel_leave_month = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    cancel_leave_day = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    cancel_leave_date = table.Column<DateTime>(type: "datetime2", maxLength: 8, nullable: false),
                    cancel_leave_approver_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    cancel_leave_purpose = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    cancel_leave_state = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    cancel_leave_approve_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelLeave", x => new { x.cancel_leave_Id, x.cancel_leave_employee_code, x.cancel_leave_year, x.cancel_leave_month, x.cancel_leave_day });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancelLeave",
                schema: "App");
        }
    }
}
