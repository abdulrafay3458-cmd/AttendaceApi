using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_reject_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "cancel_leave_reject_at",
                schema: "App",
                table: "CancelLeave",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cancel_leave_reject_reason",
                schema: "App",
                table: "CancelLeave",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cancel_leave_reject_at",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.DropColumn(
                name: "cancel_leave_reject_reason",
                schema: "App",
                table: "CancelLeave");
        }
    }
}
