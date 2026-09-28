using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedTableAttendencHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {          

            migrationBuilder.CreateTable(
                name: "AttendanceHistory",
                schema: "App",
                columns: table => new
                {
                    AttendenceHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    att_his_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    att_his_attendance_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    att_his_check_in_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    att_his_check_out_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    att_his_source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    att_his_latitude = table.Column<double>(type: "float", nullable: true),
                    att_his_longitude = table.Column<double>(type: "float", nullable: true),
                    att_his_location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    att_his_check_in_device_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    att_his_photo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceHistory", x => x.AttendenceHistoryId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceHistory",
                schema: "App");           
        }
    }
}
