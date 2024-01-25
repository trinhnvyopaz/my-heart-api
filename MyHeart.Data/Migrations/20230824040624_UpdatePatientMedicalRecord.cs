using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class UpdatePatientMedicalRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientMedicalRecords_UserReportFile_UserReportFileId",
                table: "PatientMedicalRecords");

            migrationBuilder.DropColumn(
                name: "UserReportFilesId",
                table: "PatientMedicalRecords");

            migrationBuilder.AlterColumn<int>(
                name: "UserReportFileId",
                table: "PatientMedicalRecords",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMedicalRecords_UserReportFile_UserReportFileId",
                table: "PatientMedicalRecords",
                column: "UserReportFileId",
                principalTable: "UserReportFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientMedicalRecords_UserReportFile_UserReportFileId",
                table: "PatientMedicalRecords");

            migrationBuilder.AlterColumn<int>(
                name: "UserReportFileId",
                table: "PatientMedicalRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserReportFilesId",
                table: "PatientMedicalRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMedicalRecords_UserReportFile_UserReportFileId",
                table: "PatientMedicalRecords",
                column: "UserReportFileId",
                principalTable: "UserReportFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
