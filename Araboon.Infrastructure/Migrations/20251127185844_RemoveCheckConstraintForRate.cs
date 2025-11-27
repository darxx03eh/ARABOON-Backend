using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCheckConstraintForRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                oldDefaultValue: new DateTime(2025, 11, 13, 13, 56, 6, 330, DateTimeKind.Utc).AddTicks(4280));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 27, 18, 58, 43, 504, DateTimeKind.Utc).AddTicks(2993),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 13, 13, 56, 6, 330, DateTimeKind.Utc).AddTicks(3945));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 13, 13, 56, 6, 330, DateTimeKind.Utc).AddTicks(4280),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 27, 18, 58, 43, 504, DateTimeKind.Utc).AddTicks(3265));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 13, 13, 56, 6, 330, DateTimeKind.Utc).AddTicks(3945),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 27, 18, 58, 43, 504, DateTimeKind.Utc).AddTicks(2993));

            migrationBuilder.AddCheckConstraint(
                name: "CK_Manga_Rate",
                table: "Mangas",
                sql: "[Rate] >= 0 And [Rate] <= 5");
        }
    }
}
