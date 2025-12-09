using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeNoteEnAndArForSwiperTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Swipers",
                newName: "NoteEn");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 12, 11, 26, 2, 575, DateTimeKind.Utc).AddTicks(42),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 11, 16, 55, 57, 428, DateTimeKind.Utc).AddTicks(101));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 12, 11, 26, 2, 574, DateTimeKind.Utc).AddTicks(9748),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 11, 16, 55, 57, 427, DateTimeKind.Utc).AddTicks(9846));

            migrationBuilder.AddColumn<string>(
                name: "NoteAr",
                table: "Swipers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteAr",
                table: "Swipers");

            migrationBuilder.RenameColumn(
                name: "NoteEn",
                table: "Swipers",
                newName: "Note");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 11, 16, 55, 57, 428, DateTimeKind.Utc).AddTicks(101),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 12, 11, 26, 2, 575, DateTimeKind.Utc).AddTicks(42));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 11, 16, 55, 57, 427, DateTimeKind.Utc).AddTicks(9846),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 12, 11, 26, 2, 574, DateTimeKind.Utc).AddTicks(9748));
        }
    }
}
