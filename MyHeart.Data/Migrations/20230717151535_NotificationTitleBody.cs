using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class NotificationTitleBody : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationQueue_UserDiseases_UserDiseaseId",
                table: "NotificationQueue");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_NotificationQueue_UserDiseaseId",
                table: "NotificationQueue");

            migrationBuilder.DropColumn(
                name: "UserDiseaseId",
                table: "NotificationQueue");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "NotificationQueue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "NotificationQueue",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDetail_Users_UserId",
                table: "UserDetail");

            migrationBuilder.DropIndex(
                name: "IX_UserDetail_UserId",
                table: "UserDetail");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "NotificationQueue");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "NotificationQueue");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "UserDiseaseId",
                table: "NotificationQueue",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_NotificationQueue_UserDiseaseId",
                table: "NotificationQueue",
                column: "UserDiseaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationQueue_UserDiseases_UserDiseaseId",
                table: "NotificationQueue",
                column: "UserDiseaseId",
                principalTable: "UserDiseases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
