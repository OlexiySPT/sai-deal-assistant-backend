using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Index_Firm_Name_Unique_CaseInsensitive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Firms_Name",
                table: "Firms");

            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX \"IX_Firms_Unique_Lower_Name\" ON \"Firms\" (lower(\"Name\"));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS \"IX_Firms_Unique_Lower_Name\";");

            migrationBuilder.CreateIndex(
                name: "IX_Firms_Name",
                table: "Firms",
                column: "Name");
        }
    }
}
