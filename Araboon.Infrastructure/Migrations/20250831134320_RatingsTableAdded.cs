using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RatingsTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    MangaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => new { x.UserID, x.MangaID });
                    table.ForeignKey(
                        name: "FK_Ratings_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Mangas_MangaID",
                        column: x => x.MangaID,
                        principalTable: "Mangas",
                        principalColumn: "MangaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_MangaID",
                table: "Ratings",
                column: "MangaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ratings");
        }
    }
}
