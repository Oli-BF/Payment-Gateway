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
                    id = table.Column<Guid>(nullable: false),
                    currency = table.Column<string>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    card_number = table.Column<string>(type: "varchar(100)", nullable: false),
                    card_expiry_date = table.Column<string>(type: "varchar(100)", nullable: false),
                    card_holder = table.Column<string>(type: "varchar(100)", nullable: false),
                    payment_successful = table.Column<bool>(nullable: false),
                    date_created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentRequests", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "paymentRequests",
                columns: new[] { "id", "amount", "card_holder", "card_number", "currency", "date_created", "card_expiry_date", "payment_successful" },
                values: new object[] { new Guid("3c7dea5c-fa0c-4584-b248-fd941368cea6"), 100.00m, "jSO5Pp9O2VnDGE2d9A3l3SaFJTcLhDe5BTSvnMIhv0jrUepdt/BrEM3kC0WveEJa", "dzm/k+JN4J7+OhTzNcPcWpaNdqOSdxrb+b57IQmpjwKGw4JgoXokunzZkH9zLogn", "GBP", new DateTime(2021, 3, 15, 4, 52, 26, 244, DateTimeKind.Utc).AddTicks(48), "gLn6PPOngj2SGuaAVeyw2Mvq1eNvgTmVvpHPpy37Vro=", true });

            migrationBuilder.InsertData(
                table: "paymentRequests",
                columns: new[] { "id", "amount", "card_holder", "card_number", "currency", "date_created", "card_expiry_date", "payment_successful" },
                values: new object[] { new Guid("d2e52fea-98ad-4578-a418-ce117f78f45d"), 249.99m, "JzuKbec8DYzRg9FOY6wgRKeuq6KO2reID8gFPBaSg/Q4jxL7vZnkOulELfM+hytB", "cFSxdBBweEukldHjeX1k3FlxGNK5LiW6ATSJ+BnDh1Ar4ll9UJmFV65wWBtdiAb7", "USD", new DateTime(2021, 3, 15, 4, 52, 26, 245, DateTimeKind.Utc).AddTicks(6495), "x3B6BnDk97LogNM7MQPy9VOt38kF2qGpyzjEcA43AnE=", false });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "paymentRequests");
        }
    }
}
