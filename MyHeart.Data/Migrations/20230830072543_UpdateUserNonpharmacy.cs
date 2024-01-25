using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class UpdateUserNonpharmacy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PharmacyId",
                table: "UserPharmacy",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiseasePharmacyId",
                table: "PatientMedicalExaminations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPharmacy_PharmacyId",
                table: "UserPharmacy",
                column: "PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPharmacy_Pharmacy_PharmacyId",
                table: "UserPharmacy",
                column: "PharmacyId",
                principalTable: "Pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPharmacy_Pharmacy_PharmacyId",
                table: "UserPharmacy");

            migrationBuilder.DropIndex(
                name: "IX_UserPharmacy_PharmacyId",
                table: "UserPharmacy");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "UserPharmacy");

            migrationBuilder.DropColumn(
                name: "DiseasePharmacyId",
                table: "PatientMedicalExaminations");
        }
    }
}
