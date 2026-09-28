using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_table_FaceImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "employee_face_image",
                schema: "General",
                table: "Employee",
                newName: "employee_face_embedding");

            migrationBuilder.CreateTable(
                name: "FaceImages",
                schema: "App",
                columns: table => new
                {
                    face_image_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    face_image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceImages", x => new { x.face_image_id, x.employee_code });
                    table.ForeignKey(
                        name: "FK_FaceImages_Employee_employee_code",
                        column: x => x.employee_code,
                        principalSchema: "General",
                        principalTable: "Employee",
                        principalColumn: "employee_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaceImages_employee_code",
                schema: "App",
                table: "FaceImages",
                column: "employee_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaceImages",
                schema: "App");

            migrationBuilder.RenameColumn(
                name: "employee_face_embedding",
                schema: "General",
                table: "Employee",
                newName: "employee_face_image");
        }
    }
}
