using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class NonpharmatictherapySynonyms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NonpharmaticTherapy_Synonyms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NonpharmaticTherapyId = table.Column<int>(type: "int", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsManual = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonpharmaticTherapy_Synonyms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonpharmaticTherapy_Synonyms_NonpharmaticTherapy_NonpharmaticTherapyId",
                        column: x => x.NonpharmaticTherapyId,
                        principalTable: "NonpharmaticTherapy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NonpharmaticTherapy_Synonyms_NonpharmaticTherapyId",
                table: "NonpharmaticTherapy_Synonyms",
                column: "NonpharmaticTherapyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NonpharmaticTherapy_Synonyms");
        }
    }
}
