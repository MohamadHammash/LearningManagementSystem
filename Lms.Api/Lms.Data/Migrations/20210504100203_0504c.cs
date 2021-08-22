using Microsoft.EntityFrameworkCore.Migrations;

namespace Lms.API.Data.Migrations
{
    public partial class _0504c : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorLiterature");

            migrationBuilder.CreateTable(
                name: "Authorship",
                columns: table => new
                {
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    LiteratureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorship", x => new { x.AuthorId, x.LiteratureId });
                    table.ForeignKey(
                        name: "FK_Authorship_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Authorship_Literature_LiteratureId",
                        column: x => x.LiteratureId,
                        principalTable: "Literature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authorship_LiteratureId",
                table: "Authorship",
                column: "LiteratureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authorship");

            migrationBuilder.CreateTable(
                name: "AuthorLiterature",
                columns: table => new
                {
                    AuthorsId = table.Column<int>(type: "int", nullable: false),
                    BibliographyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorLiterature", x => new { x.AuthorsId, x.BibliographyId });
                    table.ForeignKey(
                        name: "FK_AuthorLiterature_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorLiterature_Literature_BibliographyId",
                        column: x => x.BibliographyId,
                        principalTable: "Literature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorLiterature_BibliographyId",
                table: "AuthorLiterature",
                column: "BibliographyId");
        }
    }
}
