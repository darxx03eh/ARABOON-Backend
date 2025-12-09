using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCKManga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Manga_Rate",
                table: "Mangas");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Manga_Rate",
                table: "Mangas",
                sql: "[Rate] >= 0 And [Rate] <= 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Manga_Rate",
                table: "Mangas");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Manga_Rate",
                table: "Mangas",
                sql: "[Rate] >= 1 And [Rate] <= 5");
        }
    }
}
