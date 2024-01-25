using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class ProductStripeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<string>(
                name: "ProductStripeId",
                table: "Subscriptions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductStripeId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<string>(
                name: "StripeId",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
