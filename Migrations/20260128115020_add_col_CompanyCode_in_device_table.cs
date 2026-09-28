using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_col_CompanyCode_in_device_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "device_company_code",
                schema: "General",
                table: "Devices",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_device_company_code",
                schema: "General",
                table: "Devices",
                column: "device_company_code");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Company_device_company_code",
                schema: "General",
                table: "Devices",
                column: "device_company_code",
                principalSchema: "General",
                principalTable: "Company",
                principalColumn: "company_code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Company_device_company_code",
                schema: "General",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_device_company_code",
                schema: "General",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "device_company_code",
                schema: "General",
                table: "Devices");
        }
    }
}
