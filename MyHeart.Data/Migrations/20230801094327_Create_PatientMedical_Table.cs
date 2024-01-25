using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class Create_PatientMedical_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_UserDetail_Users_DoctorId",
            //     table: "UserDetail");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            // migrationBuilder.DropIndex(
            //     name: "IX_UserDetail_DoctorId",
            //     table: "UserDetail");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "IsOCRProcessed",
                table: "UserReportFile",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.CreateTable(
                name: "OCRResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OCRText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserReportFilesId = table.Column<int>(type: "int", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OCRResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientMedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserReportFilesId = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedicalRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedTextContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OCRResultId = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedTextContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessedTextContents_OCRResults_OCRResultId",
                        column: x => x.OCRResultId,
                        principalTable: "OCRResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientMedicalExaminations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientMedicalRecordId = table.Column<int>(type: "int", nullable: true),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedicalExaminations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientMedicalExaminations_PatientMedicalRecords_PatientMedicalRecordId",
                        column: x => x.PatientMedicalRecordId,
                        principalTable: "PatientMedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserReportFile_OCRResultsId",
                table: "UserReportFile",
                column: "OCRResultsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReportFile_PatientMedicalRecordId",
                table: "UserReportFile",
                column: "PatientMedicalRecordId",
                unique: true);

            // migrationBuilder.CreateIndex(
            //     name: "IX_UserDetail_UserId",
            //     table: "UserDetail",
            //     column: "UserId",
            //     unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalExaminations_PatientMedicalRecordId",
                table: "PatientMedicalExaminations",
                column: "PatientMedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedTextContents_OCRResultId",
                table: "ProcessedTextContents",
                column: "OCRResultId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_UserDetail_Users_UserId",
            //     table: "UserDetail",
            //     column: "UserId",
            //     principalTable: "Users",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDetail_Users_UserId",
                table: "UserDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReportFile_OCRResults_OCRResultsId",
                table: "UserReportFile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReportFile_PatientMedicalRecords_PatientMedicalRecordId",
                table: "UserReportFile");

            migrationBuilder.DropTable(
                name: "PatientMedicalExaminations");

            migrationBuilder.DropTable(
                name: "ProcessedTextContents");

            migrationBuilder.DropTable(
                name: "PatientMedicalRecords");

            migrationBuilder.DropTable(
                name: "OCRResults");

            migrationBuilder.DropIndex(
                name: "IX_UserReportFile_OCRResultsId",
                table: "UserReportFile");

            migrationBuilder.DropIndex(
                name: "IX_UserReportFile_PatientMedicalRecordId",
                table: "UserReportFile");

            migrationBuilder.DropIndex(
                name: "IX_UserDetail_UserId",
                table: "UserDetail");

            migrationBuilder.DropColumn(
                name: "IsOCRProcessed",
                table: "UserReportFile");

            migrationBuilder.DropColumn(
                name: "OCRResultsId",
                table: "UserReportFile");

            migrationBuilder.DropColumn(
                name: "PatientMedicalRecordId",
                table: "UserReportFile");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDetail_DoctorId",
                table: "UserDetail",
                column: "DoctorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetail_Users_DoctorId",
                table: "UserDetail",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
