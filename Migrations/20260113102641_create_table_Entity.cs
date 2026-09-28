using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class create_table_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entity",
                schema: "General",
                columns: table => new
                {
                    entity_code = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false),
                    entity_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    entity_legal_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    entity_short_name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    entity_country_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    entity_address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    entity_email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    entity_phone = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    entity_industry_code = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: true),
                    entity_fax = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    entity_gl_code = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: true),
                    entity_fiscal_year = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    latitude = table.Column<double>(type: "float", nullable: true),
                    longitude = table.Column<double>(type: "float", nullable: true),
                    entity_parent_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    entity_child_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    entity_status = table.Column<bool>(type: "bit", nullable: false),
                    entity_client_vendor = table.Column<bool>(type: "bit", nullable: false),
                    entity_allowed_radius_meters = table.Column<int>(type: "int", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entity", x => x.entity_code);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entity",
                schema: "General");
        }
    }
}
