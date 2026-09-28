using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class alter_table_task_and_notifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Task_time_log_task_id",
                schema: "App",
                table: "TaskTimeLogs");

            migrationBuilder.DropTable(
                name: "Task",
                schema: "App");

            migrationBuilder.AlterColumn<DateTime>(
                name: "notification_check_time",
                schema: "App",
                table: "Notification",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "notification_id",
                schema: "App",
                table: "Notification",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "UserTask",
                schema: "App",
                columns: table => new
                {
                    task_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    task_title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    task_description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    task_assigned_by = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    task_assigned_by_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    task_assigned_to = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    task_assigned_to_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    task_total_hours_worked = table.Column<double>(type: "float", maxLength: 50, nullable: true),
                    task_assign_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    task_due_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    task_is_active = table.Column<bool>(type: "bit", nullable: false),
                    task_is_deleted = table.Column<bool>(type: "bit", nullable: true),
                    task_priority = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    task_status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTask", x => x.task_id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_UserTask_time_log_task_id",
                schema: "App",
                table: "TaskTimeLogs",
                column: "time_log_task_id",
                principalSchema: "App",
                principalTable: "UserTask",
                principalColumn: "task_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_UserTask_time_log_task_id",
                schema: "App",
                table: "TaskTimeLogs");

            migrationBuilder.DropTable(
                name: "UserTask",
                schema: "App");

            migrationBuilder.AlterColumn<DateTime>(
                name: "notification_check_time",
                schema: "App",
                table: "Notification",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "notification_id",
                schema: "App",
                table: "Notification",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "Task",
                schema: "App",
                columns: table => new
                {
                    task_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    task_assigned_by = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    task_assigned_by_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    task_assign_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    task_assigned_to = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    task_assigned_to_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    task_description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    task_due_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    task_is_active = table.Column<bool>(type: "bit", nullable: false),
                    task_is_deleted = table.Column<bool>(type: "bit", nullable: true),
                    task_priority = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    task_status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    task_title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.task_id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Task_time_log_task_id",
                schema: "App",
                table: "TaskTimeLogs",
                column: "time_log_task_id",
                principalSchema: "App",
                principalTable: "Task",
                principalColumn: "task_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
