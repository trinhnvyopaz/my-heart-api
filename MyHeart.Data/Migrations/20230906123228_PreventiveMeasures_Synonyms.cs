using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class PreventiveMeasures_Synonyms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreventiveMeasures_Synonyms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreventiveMeasureId = table.Column<int>(type: "int", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsManual = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreventiveMeasures_Synonyms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreventiveMeasures_Synonyms_PreventiveMeasures_PreventiveMeasureId",
                        column: x => x.PreventiveMeasureId,
                        principalTable: "PreventiveMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreventiveMeasures_Synonyms_PreventiveMeasureId",
                table: "PreventiveMeasures_Synonyms",
                column: "PreventiveMeasureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreventiveMeasures_Synonyms");
        }
    }
}
