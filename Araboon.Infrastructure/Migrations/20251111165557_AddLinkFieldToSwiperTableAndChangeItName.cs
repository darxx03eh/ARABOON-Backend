using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkFieldToSwiperTableAndChangeItName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Siwipers",
                table: "Siwipers");

            migrationBuilder.RenameTable(
                name: "Siwipers",
                newName: "Swipers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 11, 16, 55, 57, 428, DateTimeKind.Utc).AddTicks(101),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 11, 10, 55, 32, 122, DateTimeKind.Utc).AddTicks(8583));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Swipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 11, 16, 55, 57, 427, DateTimeKind.Utc).AddTicks(9846),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 11, 10, 55, 32, 122, DateTimeKind.Utc).AddTicks(8102));

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Swipers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Swipers",
                table: "Swipers",
                column: "SwiperId");

            migrationBuilder.CreateIndex(
                name: "IX_Swipers_Link",
                table: "Swipers",
                column: "Link",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Swipers",
                table: "Swipers");

            migrationBuilder.DropIndex(
                name: "IX_Swipers_Link",
                table: "Swipers");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Swipers");

            migrationBuilder.RenameTable(
                name: "Swipers",
                newName: "Siwipers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Siwipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 11, 10, 55, 32, 122, DateTimeKind.Utc).AddTicks(8583),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 11, 16, 55, 57, 428, DateTimeKind.Utc).AddTicks(101));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Siwipers",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 11, 10, 55, 32, 122, DateTimeKind.Utc).AddTicks(8102),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 11, 16, 55, 57, 427, DateTimeKind.Utc).AddTicks(9846));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Siwipers",
                table: "Siwipers",
                column: "SwiperId");
        }
    }
}
