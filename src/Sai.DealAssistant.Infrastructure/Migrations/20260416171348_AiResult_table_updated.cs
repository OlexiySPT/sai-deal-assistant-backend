using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AiResult_table_updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AiResults",
                table: "AiResults");

            migrationBuilder.DropIndex(
                name: "IX_AiResults_RequestId",
                table: "AiResults");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AiResults");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AiResults",
                table: "AiResults",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AiResults",
                table: "AiResults");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AiResults",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AiResults",
                table: "AiResults",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AiResults_RequestId",
                table: "AiResults",
                column: "RequestId");
        }
    }
}
