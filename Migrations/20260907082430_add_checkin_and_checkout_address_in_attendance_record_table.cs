using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_checkin_and_checkout_address_in_attendance_record_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "att_rec_check_in_address",
                schema: "App",
                table: "AttendanceRecord",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "att_rec_check_out_address",
                schema: "App",
                table: "AttendanceRecord",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "att_rec_check_in_address",
                schema: "App",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "att_rec_check_out_address",
                schema: "App",
                table: "AttendanceRecord");
        }
    }
}
