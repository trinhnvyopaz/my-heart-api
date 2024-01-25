using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class UserStripeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeId",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeId",
                table: "Users");
        }
    }
}
