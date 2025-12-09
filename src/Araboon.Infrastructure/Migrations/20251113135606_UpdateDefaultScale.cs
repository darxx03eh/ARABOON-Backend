using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDefaultScale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                oldDefaultValue: new DateTime(2025, 11, 12, 11, 26, 2, 575, DateTimeKind.Utc).AddTicks(42));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 13, 13, 56, 6, 330, DateTimeKind.Utc).AddTicks(3945),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 12, 11, 26, 2, 574, DateTimeKind.Utc).AddTicks(9748));

            migrationBuilder.AlterColumn<decimal>(
                name: "Scale",
                table: "ProfileImages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 1.2m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 12, 11, 26, 2, 575, DateTimeKind.Utc).AddTicks(42),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 13, 13, 56, 6, 330, DateTimeKind.Utc).AddTicks(4280));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 12, 11, 26, 2, 574, DateTimeKind.Utc).AddTicks(9748),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 13, 13, 56, 6, 330, DateTimeKind.Utc).AddTicks(3945));

            migrationBuilder.AlterColumn<decimal>(
                name: "Scale",
                table: "ProfileImages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 1.2m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 1m);
        }
    }
}
