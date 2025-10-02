using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Software.Middleware.Domains.Application.Persistence.Sql.Migrations
{
    /// <inheritdoc />
    public partial class CreateApplicationScopesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationScopes",
                schema: "Core",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationScopes", x => x.Key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationApplicationScopes",
                schema: "Core",
                columns: table => new
                {
                    ApplicationEntityKey = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ScopesKey = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationApplicationScopes", x => new { x.ApplicationEntityKey, x.ScopesKey });
                    table.ForeignKey(
                        name: "FK_ApplicationApplicationScopes_ApplicationScopes_ScopesKey",
                        column: x => x.ScopesKey,
                        principalSchema: "Core",
                        principalTable: "ApplicationScopes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationApplicationScopes_Applications_ApplicationEntityK~",
                        column: x => x.ApplicationEntityKey,
                        principalSchema: "Core",
                        principalTable: "Applications",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationApplicationScopes_ScopesKey",
                schema: "Core",
                table: "ApplicationApplicationScopes",
                column: "ScopesKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationApplicationScopes",
                schema: "Core");

            migrationBuilder.DropTable(
                name: "ApplicationScopes",
                schema: "Core");
        }
    }
}
