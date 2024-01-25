using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class SymptomSynonym : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Symptom_Synonyms_Symptoms_SymptomsId",
                table: "Symptom_Synonyms");

            migrationBuilder.RenameColumn(
                name: "SymptomsId",
                table: "Symptom_Synonyms",
                newName: "SymptomId");

            migrationBuilder.RenameIndex(
                name: "IX_Symptom_Synonyms_SymptomsId",
                table: "Symptom_Synonyms",
                newName: "IX_Symptom_Synonyms_SymptomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Symptom_Synonyms_Symptoms_SymptomId",
                table: "Symptom_Synonyms",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Symptom_Synonyms_Symptoms_SymptomId",
                table: "Symptom_Synonyms");

            migrationBuilder.RenameColumn(
                name: "SymptomId",
                table: "Symptom_Synonyms",
                newName: "SymptomsId");

            migrationBuilder.RenameIndex(
                name: "IX_Symptom_Synonyms_SymptomId",
                table: "Symptom_Synonyms",
                newName: "IX_Symptom_Synonyms_SymptomsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Symptom_Synonyms_Symptoms_SymptomsId",
                table: "Symptom_Synonyms",
                column: "SymptomsId",
                principalTable: "Symptoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
