using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOverTimeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskOvertime",
                schema: "App",
                columns: table => new
                {
                    overtime_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    overtime_task_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    overtime_employee_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    overtime_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    overtime_hours = table.Column<double>(type: "float", nullable: false),
                    overtime_reason = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    overtime_status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "pending"),
                    overtime_approved_by = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    overtime_approved_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    overtime_rejection_by = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    overtime_rejection_reason = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    overtime_created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskOvertime", x => x.overtime_id);
                    table.ForeignKey(
                        name: "FK_TaskOvertime_UserTask_overtime_task_id",
                        column: x => x.overtime_task_id,
                        principalSchema: "App",
                        principalTable: "UserTask",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Cascade);
                });
      
            migrationBuilder.CreateIndex(
                name: "IX_TaskOvertime_overtime_task_id",
                schema: "App",
                table: "TaskOvertime",
                column: "overtime_task_id");
       
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskOvertime",
                schema: "App");          
        }
    }
}
