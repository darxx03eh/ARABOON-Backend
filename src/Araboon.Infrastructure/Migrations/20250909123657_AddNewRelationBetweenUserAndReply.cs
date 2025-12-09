using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araboon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewRelationBetweenUserAndReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_AspNetUsers_UserID",
                table: "Replies");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Replies",
                newName: "ToUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Replies_UserID",
                table: "Replies",
                newName: "IX_Replies_ToUserID");

            migrationBuilder.AddColumn<int>(
                name: "FromUserID",
                table: "Replies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Replies_FromUserID",
                table: "Replies",
                column: "FromUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_AspNetUsers_FromUserID",
                table: "Replies",
                column: "FromUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_AspNetUsers_ToUserID",
                table: "Replies",
                column: "ToUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_AspNetUsers_FromUserID",
                table: "Replies");

            migrationBuilder.DropForeignKey(
                name: "FK_Replies_AspNetUsers_ToUserID",
                table: "Replies");

            migrationBuilder.DropIndex(
                name: "IX_Replies_FromUserID",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "FromUserID",
                table: "Replies");

            migrationBuilder.RenameColumn(
                name: "ToUserID",
                table: "Replies",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Replies_ToUserID",
                table: "Replies",
                newName: "IX_Replies_UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_AspNetUsers_UserID",
                table: "Replies",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
