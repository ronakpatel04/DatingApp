using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class MessageEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_ReciverId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ReiverUsername",
                table: "Messages",
                newName: "ReceiverUsername");

            migrationBuilder.RenameColumn(
                name: "ReciverId",
                table: "Messages",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "ReciverDeleted",
                table: "Messages",
                newName: "ReceiverDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ReciverId",
                table: "Messages",
                newName: "IX_Messages_ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_ReceiverId",
                table: "Messages",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_ReceiverId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ReceiverUsername",
                table: "Messages",
                newName: "ReiverUsername");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Messages",
                newName: "ReciverId");

            migrationBuilder.RenameColumn(
                name: "ReceiverDeleted",
                table: "Messages",
                newName: "ReciverDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                newName: "IX_Messages_ReciverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_ReciverId",
                table: "Messages",
                column: "ReciverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
