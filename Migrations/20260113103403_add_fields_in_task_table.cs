using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_fields_in_task_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "task_entity_code",
                schema: "App",
                table: "UserTask",
                type: "varchar(4)",
                unicode: false,
                maxLength: 4,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTask_task_entity_code",
                schema: "App",
                table: "UserTask",
                column: "task_entity_code");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTask_Entity_task_entity_code",
                schema: "App",
                table: "UserTask",
                column: "task_entity_code",
                principalSchema: "General",
                principalTable: "Entity",
                principalColumn: "entity_code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTask_Entity_task_entity_code",
                schema: "App",
                table: "UserTask");

            migrationBuilder.DropIndex(
                name: "IX_UserTask_task_entity_code",
                schema: "App",
                table: "UserTask");

            migrationBuilder.DropColumn(
                name: "task_entity_code",
                schema: "App",
                table: "UserTask");
        }
    }
}
