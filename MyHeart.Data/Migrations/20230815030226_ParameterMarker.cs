using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class ParameterMarker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReportFile_OCRResults_OCRResultsId",
                table: "UserReportFile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReportFile_PatientMedicalRecords_PatientMedicalRecordId",
                table: "UserReportFile");

            migrationBuilder.DropIndex(
                name: "IX_UserReportFile_OCRResultsId",
                table: "UserReportFile");

            migrationBuilder.DropIndex(
                name: "IX_UserReportFile_PatientMedicalRecordId",
                table: "UserReportFile");

            migrationBuilder.DropColumn(
                name: "OCRResultsId",
                table: "UserReportFile");

            migrationBuilder.DropColumn(
                name: "PatientMedicalRecordId",
                table: "UserReportFile");

            migrationBuilder.AddColumn<int>(
                name: "UserReportFileId",
                table: "PatientMedicalRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Parameter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConformityType = table.Column<int>(type: "int", nullable: false),
                    SearchType = table.Column<int>(type: "int", nullable: false),
                    SearchValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Marker_Parameter_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "Parameter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalRecords_UserReportFileId",
                table: "PatientMedicalRecords",
                column: "UserReportFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Marker_ParameterId",
                table: "Marker",
                column: "ParameterId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMedicalRecords_UserReportFile_UserReportFileId",
                table: "PatientMedicalRecords",
                column: "UserReportFileId",
                principalTable: "UserReportFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientMedicalRecords_UserReportFile_UserReportFileId",
                table: "PatientMedicalRecords");

            migrationBuilder.DropTable(
                name: "Marker");

            migrationBuilder.DropTable(
                name: "Parameter");

            migrationBuilder.DropIndex(
                name: "IX_PatientMedicalRecords_UserReportFileId",
                table: "PatientMedicalRecords");

            migrationBuilder.DropColumn(
                name: "UserReportFileId",
                table: "PatientMedicalRecords");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PatientMedicalExaminations");

            migrationBuilder.AddColumn<int>(
                name: "OCRResultsId",
                table: "UserReportFile",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientMedicalRecordId",
                table: "UserReportFile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserReportFile_OCRResultsId",
                table: "UserReportFile",
                column: "OCRResultsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReportFile_PatientMedicalRecordId",
                table: "UserReportFile",
                column: "PatientMedicalRecordId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReportFile_OCRResults_OCRResultsId",
                table: "UserReportFile",
                column: "OCRResultsId",
                principalTable: "OCRResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReportFile_PatientMedicalRecords_PatientMedicalRecordId",
                table: "UserReportFile",
                column: "PatientMedicalRecordId",
                principalTable: "PatientMedicalRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
