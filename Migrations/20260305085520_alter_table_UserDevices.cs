using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class alter_table_UserDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "App",
                table: "UserDevices");

            migrationBuilder.AddColumn<string>(
                name: "approved_by",
                schema: "App",
                table: "UserDevices",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rejected_by",
                schema: "App",
                table: "UserDevices",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "approved_by",
                schema: "App",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "rejected_by",
                schema: "App",
                table: "UserDevices");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "App",
                table: "UserDevices",
                type: "datetime2",
                nullable: true);
        }
    }
}
