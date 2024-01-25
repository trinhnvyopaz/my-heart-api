using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class DiseaseSynonymsProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DiseaseSynonyms_DiseaseId",
                table: "DiseaseSynonyms",
                column: "DiseaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiseaseSynonyms_Disease_DiseaseId",
                table: "DiseaseSynonyms",
                column: "DiseaseId",
                principalTable: "Disease",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiseaseSynonyms_Disease_DiseaseId",
                table: "DiseaseSynonyms");

            migrationBuilder.DropIndex(
                name: "IX_DiseaseSynonyms_DiseaseId",
                table: "DiseaseSynonyms");
        }
    }
}
