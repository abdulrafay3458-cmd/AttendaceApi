using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class addTableVerify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserVerifyOPT",
                schema: "App",
                columns: table => new
                {
                    user_verify_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    user_verify_email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    user_verify_otp = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVerifyOPT", x => x.user_verify_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVerifyOPT",
                schema: "App");
        }
    }
}
