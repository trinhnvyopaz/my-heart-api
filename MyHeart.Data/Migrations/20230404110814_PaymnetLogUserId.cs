using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class PaymnetLogUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PaymentLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLog_UserId",
                table: "PaymentLog",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentLog_Users_UserId",
                table: "PaymentLog",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentLog_Users_UserId",
                table: "PaymentLog");

            migrationBuilder.DropIndex(
                name: "IX_PaymentLog_UserId",
                table: "PaymentLog");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PaymentLog");
        }
    }
}
