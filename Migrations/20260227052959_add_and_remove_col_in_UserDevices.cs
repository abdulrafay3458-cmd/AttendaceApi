using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_and_remove_col_in_UserDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modified_at",
                schema: "App",
                table: "UserDevices");

            migrationBuilder.AddColumn<string>(
                name: "device_manufacturer",
                schema: "App",
                table: "UserDevices",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "device_model",
                schema: "App",
                table: "UserDevices",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "device_manufacturer",
                schema: "App",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "device_model",
                schema: "App",
                table: "UserDevices");

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                schema: "App",
                table: "UserDevices",
                type: "datetime2",
                nullable: true);
        }
    }
}
