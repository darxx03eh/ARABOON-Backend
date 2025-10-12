using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderToOrderImageAndAddIndexingForChapter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chapters_MangaID",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "EnglishChapterImages",
                newName: "OrderImage");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "ArabicChapterImages",
                newName: "OrderImage");

            migrationBuilder.CreateIndex(
                name: "IX_MangaId_ChapterNo",
                table: "Chapters",
                columns: new[] { "MangaID", "ChapterNo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MangaId_ChapterNo",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "OrderImage",
                table: "EnglishChapterImages",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "OrderImage",
                table: "ArabicChapterImages",
                newName: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_MangaID",
                table: "Chapters",
                column: "MangaID");
        }
    }
}
