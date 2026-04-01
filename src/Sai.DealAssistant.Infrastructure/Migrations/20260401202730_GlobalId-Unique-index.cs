using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GlobalIdUniqueindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_GlobalId",
                table: "Users",
                column: "GlobalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Firms_GlobalId",
                table: "Firms",
                column: "GlobalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_GlobalId",
                table: "Events",
                column: "GlobalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_GlobalId",
                table: "Deals",
                column: "GlobalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_GlobalId",
                table: "ContactPersons",
                column: "GlobalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_GlobalId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Firms_GlobalId",
                table: "Firms");

            migrationBuilder.DropIndex(
                name: "IX_Events_GlobalId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Deals_GlobalId",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_ContactPersons_GlobalId",
                table: "ContactPersons");
        }
    }
}
