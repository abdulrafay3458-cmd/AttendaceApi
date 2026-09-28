using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class addTableRegistrationImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "registration_image",
                schema: "App",
                columns: table => new
                {
                    registration_image_code = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    registration_image_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    registration_imaget_admin_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    registration_image_request_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    registration_image_approve_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    registration_image_rejection_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    registration_image_rejection_reason = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    registration_image_face_image = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    registration_image_status = table.Column<bool>(type: "bit", nullable: false),
                    registration_image_history_status = table.Column<bool>(type: "bit", nullable: false),
                    registration_image_company_code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registration_image", x => x.registration_image_code);
                    table.ForeignKey(
                        name: "FK_registration_image_Employee_registration_image_employee_code",
                        column: x => x.registration_image_employee_code,
                        principalSchema: "General",
                        principalTable: "Employee",
                        principalColumn: "employee_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_registration_image_registration_image_employee_code",
                schema: "App",
                table: "registration_image",
                column: "registration_image_employee_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registration_image",
                schema: "App");
        }
    }
}
