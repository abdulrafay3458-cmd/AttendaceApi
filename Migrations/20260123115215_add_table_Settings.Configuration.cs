using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_table_SettingsConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                schema: "Settings",
                columns: table => new
                {
                    configuration_heading = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    configuration_title = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    configuration_value_type = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: false),
                    configuration_value = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => new { x.configuration_heading, x.configuration_title });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration",
                schema: "Settings");
        }
    }
}
