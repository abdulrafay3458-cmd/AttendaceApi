using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class removeAutoIncreamentleaveRequisitionMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequisitionMaster",
                schema: "App",
                table: "LeaveRequisitionMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequisitionDetail",
                schema: "App",
                table: "LeaveRequisitionDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequisitionBalance",
                schema: "App",
                table: "LeaveRequisitionBalance");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "App",
                table: "LeaveRequisitionMaster");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "App",
                table: "LeaveRequisitionDetail");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "App",
                table: "LeaveRequisitionBalance");

            migrationBuilder.AddColumn<int>(
                name: "leave_requisition_master_id",
                schema: "App",
                table: "LeaveRequisitionMaster",
                type: "int",
                maxLength: 4,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "leave_requisition_detail_id",
                schema: "App",
                table: "LeaveRequisitionDetail",
                type: "int",
                maxLength: 4,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "leave_requisition_balance_id",
                schema: "App",
                table: "LeaveRequisitionBalance",
                type: "int",
                maxLength: 4,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequisitionMaster",
                schema: "App",
                table: "LeaveRequisitionMaster",
                column: "leave_requisition_master_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequisitionDetail",
                schema: "App",
                table: "LeaveRequisitionDetail",
                column: "leave_requisition_detail_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequisitionBalance",
                schema: "App",
                table: "LeaveRequisitionBalance",
                column: "leave_requisition_balance_id");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequisitionDetail_leave_requisition_detail_master_id",
                schema: "App",
                table: "LeaveRequisitionDetail",
                column: "leave_requisition_detail_master_id");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequisitionBalance_leave_requisition_balance_master_id",
                schema: "App",
                table: "LeaveRequisitionBalance",
                column: "leave_requisition_balance_master_id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequisitionBalance_LeaveRequisitionMaster_leave_requisition_balance_master_id",
                schema: "App",
                table: "LeaveRequisitionBalance",
                column: "leave_requisition_balance_master_id",
                principalSchema: "App",
                principalTable: "LeaveRequisitionMaster",
                principalColumn: "leave_requisition_master_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequisitionDetail_LeaveRequisitionMaster_leave_requisition_detail_master_id",
                schema: "App",
                table: "LeaveRequisitionDetail",
                column: "leave_requisition_detail_master_id",
                principalSchema: "App",
                principalTable: "LeaveRequisitionMaster",
                principalColumn: "leave_requisition_master_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequisitionBalance_LeaveRequisitionMaster_leave_requisition_balance_master_id",
                schema: "App",
                table: "LeaveRequisitionBalance");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequisitionDetail_LeaveRequisitionMaster_leave_requisition_detail_master_id",
                schema: "App",
                table: "LeaveRequisitionDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequisitionMaster",
                schema: "App",
                table: "LeaveRequisitionMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequisitionDetail",
                schema: "App",
                table: "LeaveRequisitionDetail");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequisitionDetail_leave_requisition_detail_master_id",
                schema: "App",
                table: "LeaveRequisitionDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveRequisitionBalance",
                schema: "App",
                table: "LeaveRequisitionBalance");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequisitionBalance_leave_requisition_balance_master_id",
                schema: "App",
                table: "LeaveRequisitionBalance");

            migrationBuilder.DropColumn(
                name: "leave_requisition_master_id",
                schema: "App",
                table: "LeaveRequisitionMaster");

            migrationBuilder.DropColumn(
                name: "leave_requisition_detail_id",
                schema: "App",
                table: "LeaveRequisitionDetail");

            migrationBuilder.DropColumn(
                name: "leave_requisition_balance_id",
                schema: "App",
                table: "LeaveRequisitionBalance");

            migrationBuilder.AddColumn<int>(
                name: "Code",
                schema: "App",
                table: "LeaveRequisitionMaster",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Code",
                schema: "App",
                table: "LeaveRequisitionDetail",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Code",
                schema: "App",
                table: "LeaveRequisitionBalance",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequisitionMaster",
                schema: "App",
                table: "LeaveRequisitionMaster",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequisitionDetail",
                schema: "App",
                table: "LeaveRequisitionDetail",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveRequisitionBalance",
                schema: "App",
                table: "LeaveRequisitionBalance",
                column: "Code");
        }
    }
}
