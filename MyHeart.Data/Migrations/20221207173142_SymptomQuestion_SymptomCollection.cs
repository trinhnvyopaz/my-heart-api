using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class SymptomQuestion_SymptomCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SymptomQuestions_Symptom_SymptomQuestionsId",
                table: "SymptomQuestions_Symptom");

            migrationBuilder.CreateIndex(
                name: "IX_SymptomQuestions_Symptom_SymptomQuestionsId",
                table: "SymptomQuestions_Symptom",
                column: "SymptomQuestionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SymptomQuestions_Symptom_SymptomQuestionsId",
                table: "SymptomQuestions_Symptom");

            migrationBuilder.CreateIndex(
                name: "IX_SymptomQuestions_Symptom_SymptomQuestionsId",
                table: "SymptomQuestions_Symptom",
                column: "SymptomQuestionsId",
                unique: true);
        }
    }
}
