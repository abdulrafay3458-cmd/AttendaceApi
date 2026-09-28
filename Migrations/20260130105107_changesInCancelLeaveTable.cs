using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class changesInCancelLeaveTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CancelLeave",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.DropColumn(
                name: "cancel_leave_year",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.DropColumn(
                name: "cancel_leave_month",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.DropColumn(
                name: "cancel_leave_day",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.DropColumn(
                name: "cancel_leave_date",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.AddColumn<DateTime>(
                name: "cancel_leave_from_date",
                schema: "App",
                table: "CancelLeave",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "cancel_leave_to_date",
                schema: "App",
                table: "CancelLeave",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CancelLeave",
                schema: "App",
                table: "CancelLeave",
                columns: new[] { "cancel_leave_Id", "cancel_leave_employee_code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CancelLeave",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.DropColumn(
                name: "cancel_leave_from_date",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.DropColumn(
                name: "cancel_leave_to_date",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.AddColumn<int>(
                name: "cancel_leave_year",
                schema: "App",
                table: "CancelLeave",
                type: "int",
                maxLength: 4,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cancel_leave_month",
                schema: "App",
                table: "CancelLeave",
                type: "int",
                maxLength: 4,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cancel_leave_day",
                schema: "App",
                table: "CancelLeave",
                type: "int",
                maxLength: 4,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "cancel_leave_date",
                schema: "App",
                table: "CancelLeave",
                type: "datetime2",
                maxLength: 8,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CancelLeave",
                schema: "App",
                table: "CancelLeave",
                columns: new[] { "cancel_leave_Id", "cancel_leave_employee_code", "cancel_leave_year", "cancel_leave_month", "cancel_leave_day" });
        }
    }
}
