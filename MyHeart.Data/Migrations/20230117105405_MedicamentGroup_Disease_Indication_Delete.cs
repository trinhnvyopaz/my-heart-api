using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class MedicamentGroup_Disease_Indication_Delete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disease_MedicamentGroup_MedicamentGroup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Disease_MedicamentGroup_MedicamentGroup",
                columns: table => new
                {
                    DiseaseId = table.Column<int>(type: "int", nullable: false),
                    MedicamentGroupId = table.Column<int>(type: "int", nullable: false),
                    BondStrength = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "UNDEFINED")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disease_MedicamentGroup_MedicamentGroup", x => new { x.DiseaseId, x.MedicamentGroupId });
                    table.ForeignKey(
                        name: "FK_Disease_MedicamentGroup_MedicamentGroup_Disease_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Disease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Disease_MedicamentGroup_MedicamentGroup_MedicamentGroup_MedicamentGroupId",
                        column: x => x.MedicamentGroupId,
                        principalTable: "MedicamentGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disease_MedicamentGroup_MedicamentGroup_MedicamentGroupId",
                table: "Disease_MedicamentGroup_MedicamentGroup",
                column: "MedicamentGroupId");
        }
    }
}
