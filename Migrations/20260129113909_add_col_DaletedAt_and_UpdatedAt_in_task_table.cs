using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_col_DaletedAt_and_UpdatedAt_in_task_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "task_updated_at",
                schema: "App",
                table: "UserTask",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "task_deleted_at",
                schema: "App",
                table: "UserTask",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "task_updated_at",
                schema: "App",
                table: "UserTask");

            migrationBuilder.DropColumn(
                name: "task_deleted_at",
                schema: "App",
                table: "UserTask");
        }
    }
}
