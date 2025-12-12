using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ListingWebApp.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "CurrencyCode", "Description", "IconKey", "IsTransferAllowed", "MaxTransferAmount", "MinTransferAmount", "Name" },
                values: new object[,]
                {
                    { "EUR", "Eurozone settlement currency", "currency-eur", true, 1000000m, 1m, "Euro" },
                    { "RUB", "Base currency for local operations", "currency-rub", true, 1000000m, 1m, "Russian Ruble" },
                    { "USD", "International settlement currency", "currency-usd", true, 1000000m, 1m, "US Dollar" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: "EUR");

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: "RUB");

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "CurrencyCode",
                keyValue: "USD");
        }
    }
}
