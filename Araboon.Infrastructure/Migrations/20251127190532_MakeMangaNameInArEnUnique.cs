using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeMangaNameInArEnUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 27, 19, 5, 31, 819, DateTimeKind.Utc).AddTicks(951),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 27, 18, 58, 43, 504, DateTimeKind.Utc).AddTicks(3265));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 27, 19, 5, 31, 819, DateTimeKind.Utc).AddTicks(635),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 27, 18, 58, 43, 504, DateTimeKind.Utc).AddTicks(2993));

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

            migrationBuilder.AddCheckConstraint(
                name: "CK_Manga_Rate",
                table: "Mangas",
                sql: "[Rate] >= 0 And [Rate] <= 5");
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

            migrationBuilder.DropCheckConstraint(
                name: "CK_Manga_Rate",
                table: "Mangas");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 27, 18, 58, 43, 504, DateTimeKind.Utc).AddTicks(3265),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 27, 19, 5, 31, 819, DateTimeKind.Utc).AddTicks(951));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 27, 18, 58, 43, 504, DateTimeKind.Utc).AddTicks(2993),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 27, 19, 5, 31, 819, DateTimeKind.Utc).AddTicks(635));

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
        }
    }
}
