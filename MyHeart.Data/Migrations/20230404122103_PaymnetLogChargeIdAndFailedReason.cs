using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class PaymnetLogChargeIdAndFailedReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "PaymentLog",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "ChargeId",
                table: "PaymentLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "PaymentLog",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargeId",
                table: "PaymentLog");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "PaymentLog");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "PaymentLog",
                newName: "PaymentIntentId");
        }
    }
}
