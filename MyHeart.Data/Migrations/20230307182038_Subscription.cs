using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class Subscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscription",
                table: "UserDetail");

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateUser = table.Column<string>(nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "LastUpdateDate", "Name", "Price" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Základ", 0m });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "LastUpdateDate", "Name", "Price" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kompletní péče", 199m });

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "UserDetail",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_UserDetail_SubscriptionId",
                table: "UserDetail",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetail_Subscriptions_SubscriptionId",
                table: "UserDetail",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDetail_Subscriptions_SubscriptionId",
                table: "UserDetail");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserDetail_SubscriptionId",
                table: "UserDetail");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "UserDetail");

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscription",
                table: "UserDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
