using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class NotificationQeueuUserIdFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueue_UserId",
                table: "NotificationQueue",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationQueue_Users_UserId",
                table: "NotificationQueue",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationQueue_Users_UserId",
                table: "NotificationQueue");

            migrationBuilder.DropIndex(
                name: "IX_NotificationQueue_UserId",
                table: "NotificationQueue");
        }
    }
}
