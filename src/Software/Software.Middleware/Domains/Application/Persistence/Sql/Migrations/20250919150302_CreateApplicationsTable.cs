using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Software.Middleware.Domains.Application.Persistence.Sql.Migrations
{
    /// <inheritdoc />
    public partial class CreateApplicationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Core");

            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "Core",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthSecret = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Iv = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_AuthId",
                schema: "Core",
                table: "Applications",
                column: "AuthId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_AuthSecret",
                schema: "Core",
                table: "Applications",
                column: "AuthSecret",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Iv",
                schema: "Core",
                table: "Applications",
                column: "Iv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Key",
                schema: "Core",
                table: "Applications",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Source",
                schema: "Core",
                table: "Applications",
                column: "Source");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications",
                schema: "Core");
        }
    }
}
