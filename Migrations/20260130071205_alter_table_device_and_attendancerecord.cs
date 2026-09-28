using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class alter_table_device_and_attendancerecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "att_rec_device_id",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "att_rec_device_name",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "att_rec_location",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Devices",
                schema: "General",
                table: "Devices");

            migrationBuilder.AlterColumn<Guid>(
                name: "device_id",
                schema: "General",
                table: "Devices",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Devices",
                schema: "General",
                table: "Devices",
                column: "device_id");

            migrationBuilder.AddColumn<Guid>(
                name: "att_rec_check_in_device_id",
                schema: "App",
                table: "AttendanceRecord",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "att_rec_check_out_device_id",
                schema: "App",
                table: "AttendanceRecord",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_att_rec_check_in_device_id",
                schema: "App",
                table: "AttendanceRecord",
                column: "att_rec_check_in_device_id");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_att_rec_check_out_device_id",
                schema: "App",
                table: "AttendanceRecord",
                column: "att_rec_check_out_device_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecord_Devices_att_rec_check_in_device_id",
                schema: "App",
                table: "AttendanceRecord",
                column: "att_rec_check_in_device_id",
                principalSchema: "General",
                principalTable: "Devices",
                principalColumn: "device_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecord_Devices_att_rec_check_out_device_id",
                schema: "App",
                table: "AttendanceRecord",
                column: "att_rec_check_out_device_id",
                principalSchema: "General",
                principalTable: "Devices",
                principalColumn: "device_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecord_Devices_att_rec_check_in_device_id",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecord_Devices_att_rec_check_out_device_id",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecord_att_rec_check_in_device_id",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecord_att_rec_check_out_device_id",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "att_rec_check_in_device_id",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "att_rec_check_out_device_id",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.AlterColumn<string>(
                name: "device_id",
                schema: "General",
                table: "Devices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "att_rec_device_id",
                schema: "App",
                table: "AttendanceRecord",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "att_rec_device_name",
                schema: "App",
                table: "AttendanceRecord",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "att_rec_location",
                schema: "App",
                table: "AttendanceRecord",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
