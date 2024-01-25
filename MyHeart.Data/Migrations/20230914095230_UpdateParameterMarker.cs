using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class UpdateParameterMarker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "PatientMedicalExaminations",
                newName: "SearchAreaReal");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "PatientMedicalExaminations",
                newName: "ResultExclusionReally");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Parameter",
                newName: "ResultFormat");

            migrationBuilder.RenameColumn(
                name: "SearchValue",
                table: "Parameter",
                newName: "ResultExclusion");

            migrationBuilder.AddColumn<bool>(
                name: "IsDataManager",
                table: "PatientMedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AfterMarkerReally",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BeforeMarkerReally",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateMined",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateMinedArea",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateMinedReal",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoseMined",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoseMinedRelly",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoseMinedSource",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoseUnitMined",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrequencyMined",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemMined",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeywordRelly",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoteMined",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PathSaving",
                table: "PatientMedicalExaminations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "Parameter",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultCondition",
                table: "Parameter",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MakerGroup",
                table: "Marker",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDataManager",
                table: "PatientMedicalRecords");

            migrationBuilder.DropColumn(
                name: "AfterMarkerReally",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "BeforeMarkerReally",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "DateMined",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "DateMinedArea",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "DateMinedReal",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "DoseMined",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "DoseMinedRelly",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "DoseMinedSource",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "DoseUnitMined",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "FrequencyMined",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "ItemMined",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "KeywordRelly",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "NoteMined",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "PathSaving",
                table: "PatientMedicalExaminations");

            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "Parameter");

            migrationBuilder.DropColumn(
                name: "ResultCondition",
                table: "Parameter");

            migrationBuilder.DropColumn(
                name: "MakerGroup",
                table: "Marker");

            migrationBuilder.RenameColumn(
                name: "SearchAreaReal",
                table: "PatientMedicalExaminations",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "ResultExclusionReally",
                table: "PatientMedicalExaminations",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ResultFormat",
                table: "Parameter",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "ResultExclusion",
                table: "Parameter",
                newName: "SearchValue");
        }
    }
}
