using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Software.Middleware.Domains.Application.Persistence.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationScopesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationEntityApplicationScopeEntity",
                schema: "Core",
                columns: table => new
                {
                    ApplicationEntityKey = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ScopesKey = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationEntityApplicationScopeEntity", x => new { x.ApplicationEntityKey, x.ScopesKey });
                    table.ForeignKey(
                        name: "FK_ApplicationEntityApplicationScopeEntity_ApplicationScopes_Sc~",
                        column: x => x.ScopesKey,
                        principalSchema: "Core",
                        principalTable: "ApplicationScopes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationEntityApplicationScopeEntity_Applications_Applica~",
                        column: x => x.ApplicationEntityKey,
                        principalSchema: "Core",
                        principalTable: "Applications",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationEntityApplicationScopeEntity_ScopesKey",
                schema: "Core",
                table: "ApplicationEntityApplicationScopeEntity",
                column: "ScopesKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationEntityApplicationScopeEntity",
                schema: "Core");
        }
    }
}
