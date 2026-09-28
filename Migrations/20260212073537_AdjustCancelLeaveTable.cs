using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdjustCancelLeaveTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cancel_leave_from_date",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.RenameColumn(
                name: "cancel_leave_to_date",
                schema: "App",
                table: "CancelLeave",
                newName: "leave_date");

            migrationBuilder.AddColumn<int>(
                name: "LeaveId",
                schema: "App",
                table: "CancelLeave",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CancelLeave_LeaveId",
                schema: "App",
                table: "CancelLeave",
                column: "LeaveId");

            migrationBuilder.AddForeignKey(
                name: "FK_CancelLeave_LeaveRequisitionMaster_LeaveId",
                schema: "App",
                table: "CancelLeave",
                column: "LeaveId",
                principalSchema: "App",
                principalTable: "LeaveRequisitionMaster",
                principalColumn: "leave_requisition_master_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancelLeave_LeaveRequisitionMaster_LeaveId",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.DropIndex(
                name: "IX_CancelLeave_LeaveId",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.DropColumn(
                name: "LeaveId",
                schema: "App",
                table: "CancelLeave");

            migrationBuilder.RenameColumn(
                name: "leave_date",
                schema: "App",
                table: "CancelLeave",
                newName: "cancel_leave_to_date");

            migrationBuilder.AddColumn<DateTime>(
                name: "cancel_leave_from_date",
                schema: "App",
                table: "CancelLeave",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
