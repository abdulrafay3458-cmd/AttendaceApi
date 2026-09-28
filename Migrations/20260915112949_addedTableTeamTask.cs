using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedTableTeamTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamTask",
                schema: "App",
                columns: table => new
                {
                    TeamTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamTask", x => new { x.TeamTaskId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_TeamTask_UserTask_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "App",
                        principalTable: "UserTask",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamTask_TaskId",
                schema: "App",
                table: "TeamTask",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamTask",
                schema: "App");
        }
    }
}
