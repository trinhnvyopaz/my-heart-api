using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class RenameCreatorType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorType",
                table: "UserAnamnesis");

            migrationBuilder.AddColumn<int>(
                name: "IsPersonal_CreatorType",
                table: "UserAnamnesis",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPersonal_CreatorType",
                table: "UserAnamnesis");

            migrationBuilder.AddColumn<int>(
                name: "CreatorType",
                table: "UserAnamnesis",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
