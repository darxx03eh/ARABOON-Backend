using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeNamesUniqueInCategoriesAndMangas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MangaNameEn",
                table: "Mangas",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MangaNameAr",
                table: "Mangas",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryNameEn",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryNameAr",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_MangaNameAr",
                table: "Mangas",
                column: "MangaNameAr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_MangaNameEn",
                table: "Mangas",
                column: "MangaNameEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryNameAr",
                table: "Categories",
                column: "CategoryNameAr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryNameEn",
                table: "Categories",
                column: "CategoryNameEn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Mangas_MangaNameAr",
                table: "Mangas");

            migrationBuilder.DropIndex(
                name: "IX_Mangas_MangaNameEn",
                table: "Mangas");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryNameAr",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryNameEn",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "MangaNameEn",
                table: "Mangas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "MangaNameAr",
                table: "Mangas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryNameEn",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryNameAr",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
