using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DealEntityExtended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmountTypeId",
                table: "Deals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Deals",
                type: "varchar",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateToEur",
                table: "Deals",
                type: "numeric(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxClientAmount",
                table: "Deals",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinClientAmount",
                table: "Deals",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProposalAmount",
                table: "Deals",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AmountTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "varchar", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmountTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deals_AmountTypeId",
                table: "Deals",
                column: "AmountTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_AmountTypes_AmountTypeId",
                table: "Deals",
                column: "AmountTypeId",
                principalTable: "AmountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_AmountTypes_AmountTypeId",
                table: "Deals");

            migrationBuilder.DropTable(
                name: "AmountTypes");

            migrationBuilder.DropIndex(
                name: "IX_Deals_AmountTypeId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "AmountTypeId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "ExchangeRateToEur",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "MaxClientAmount",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "MinClientAmount",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "ProposalAmount",
                table: "Deals");
        }
    }
}
