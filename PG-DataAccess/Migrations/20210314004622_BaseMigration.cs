using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PG_DataAccess.Migrations
{
    public partial class BaseMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "paymentRequests",
                columns: table => new
                {
                    paymentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    currency = table.Column<string>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    cardNumberMasked = table.Column<string>(type: "varchar(8000)", nullable: false),
                    expiryDate = table.Column<string>(type: "varchar(8000)", nullable: false),
                    cardHolder = table.Column<string>(type: "varchar(8000)", nullable: false),
                    paymentSuccessful = table.Column<bool>(nullable: false),
                    dateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentRequests", x => x.paymentId);
                });

            migrationBuilder.InsertData(
                table: "paymentRequests",
                columns: new[] { "paymentId", "amount", "cardHolder", "cardNumberMasked", "currency", "dateCreated", "expiryDate", "paymentSuccessful" },
                values: new object[] { 1, 100.00m, "OGLSp/L8JZtj08SahYW4CmFh1JXHE771LF0Pjf1KpVo7d0Zy/XXke1Wd7NRp8z4X", "/hsshElni3zRxnaku770aKj1WBbcYUyBYbX0LQZiLbCi8f0+A5AXtn2hxn2n40R8", "GBP", new DateTime(2021, 3, 14, 0, 46, 21, 785, DateTimeKind.Utc).AddTicks(8272), "K3MaTyLfGhQyfGUUBJelEGvHWnfo/02MbdLxRki78qA=", true });

            migrationBuilder.InsertData(
                table: "paymentRequests",
                columns: new[] { "paymentId", "amount", "cardHolder", "cardNumberMasked", "currency", "dateCreated", "expiryDate", "paymentSuccessful" },
                values: new object[] { 2, 250.00m, "C7Gpd9WgZZ7hBoF4XdEjpJMjmwLhhP0JcikPCHCTdgoDc1Kam8th/X4fCUXfFAg1", "YiP00OjmixHRcnRDHPOMmobzMRwt1OCGaQOOW0IofyvPhogZFz0EA0LnAwxWgatT", "GBP", new DateTime(2021, 3, 14, 0, 46, 21, 787, DateTimeKind.Utc).AddTicks(2912), "XVD+G/63IAXsPpbhH6Pwm1Alejd+TmbSoyha6QDpkOA=", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "paymentRequests");
        }
    }
}
