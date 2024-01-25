using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class FixSymptomSynonym : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Symptom_Synonyms_Symptoms_SymptomId",
                table: "Symptom_Synonyms");

            migrationBuilder.DropColumn(
                name: "SynonymId",
                table: "Symptom_Synonyms");

            migrationBuilder.AlterColumn<int>(
                name: "SymptomId",
                table: "Symptom_Synonyms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Symptom_Synonyms_Symptoms_SymptomId",
                table: "Symptom_Synonyms",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Symptom_Synonyms_Symptoms_SymptomId",
                table: "Symptom_Synonyms");

            migrationBuilder.AlterColumn<int>(
                name: "SymptomId",
                table: "Symptom_Synonyms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SynonymId",
                table: "Symptom_Synonyms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Symptom_Synonyms_Symptoms_SymptomId",
                table: "Symptom_Synonyms",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
