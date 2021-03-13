using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PG_DataAccess.Migrations
{
    public partial class AddPaymentRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "paymentRequests",
                columns: table => new
                {
                    paymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    cardNumberMasked = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    expiryDate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    cardHolder = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    paymentSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentRequests", x => x.paymentId);
                });

            migrationBuilder.InsertData(
                table: "paymentRequests",
                columns: new[] { "paymentId", "amount", "cardHolder", "cardNumberMasked", "currency", "dateCreated", "expiryDate", "paymentSuccessful" },
                values: new object[] { new Guid("c0bea600-d903-4fad-ba5a-894447f0fdfe"), 100.00m, "MR JOHN HAMILTON-SMITH", "************1234", "GBP", new DateTime(2021, 3, 12, 22, 7, 17, 255, DateTimeKind.Utc).AddTicks(5316), "06/21", true });

            migrationBuilder.InsertData(
                table: "paymentRequests",
                columns: new[] { "paymentId", "amount", "cardHolder", "cardNumberMasked", "currency", "dateCreated", "expiryDate", "paymentSuccessful" },
                values: new object[] { new Guid("45dc7403-1bc4-4b49-982f-466bbca8ba73"), 250.00m, "MS JANE HAMILTON-SMITH", "************5678", "GBP", new DateTime(2021, 3, 12, 22, 7, 17, 257, DateTimeKind.Utc).AddTicks(272), "06/21", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "paymentRequests");
        }
    }
}
