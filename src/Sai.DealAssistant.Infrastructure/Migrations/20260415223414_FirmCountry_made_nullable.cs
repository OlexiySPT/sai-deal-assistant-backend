using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirmCountry_made_nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiResults_Deals_DealId",
                table: "AiResults");

            migrationBuilder.DropIndex(
                name: "IX_AiResults_DealId",
                table: "AiResults");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Firms",
                type: "varchar",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "DealId",
                table: "AiResults",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Firms",
                type: "varchar",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DealId",
                table: "AiResults",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiResults_DealId",
                table: "AiResults",
                column: "DealId");

            migrationBuilder.AddForeignKey(
                name: "FK_AiResults_Deals_DealId",
                table: "AiResults",
                column: "DealId",
                principalTable: "Deals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
