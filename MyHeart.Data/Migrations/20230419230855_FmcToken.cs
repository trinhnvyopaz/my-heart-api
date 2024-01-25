using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class FmcToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFmcTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFmcTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFmcTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFmcTokens_UserId",
                table: "UserFmcTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFmcTokens");
        }
    }
}
