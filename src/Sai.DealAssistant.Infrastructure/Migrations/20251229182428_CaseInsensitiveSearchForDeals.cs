using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CaseInsensitiveSearchForDeals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Deals",
                type: "citext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_Lower90_Industry",
                table: "Deals",
                column: "Industry");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_Lower90_Name",
                table: "Deals",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Deals_Lower90_Industry",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_Lower90_Name",
                table: "Deals");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Deals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true);
        }
    }
}
