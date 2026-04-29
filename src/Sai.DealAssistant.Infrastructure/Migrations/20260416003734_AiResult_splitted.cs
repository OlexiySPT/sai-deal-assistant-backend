using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AiResult_splitted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DealId",
                table: "AiResults");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "AiResults");

            migrationBuilder.DropColumn(
                name: "Prompt",
                table: "AiResults");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AiResults");

            migrationBuilder.AddColumn<double>(
                name: "DurationSeconds",
                table: "AiResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "AiResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "AiResults",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AIRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Prompt = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    DealId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiResults_RequestId",
                table: "AiResults",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_AiResults_AIRequests_RequestId",
                table: "AiResults",
                column: "RequestId",
                principalTable: "AIRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiResults_AIRequests_RequestId",
                table: "AiResults");

            migrationBuilder.DropTable(
                name: "AIRequests");

            migrationBuilder.DropIndex(
                name: "IX_AiResults_RequestId",
                table: "AiResults");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "AiResults");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "AiResults");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "AiResults");

            migrationBuilder.AddColumn<int>(
                name: "DealId",
                table: "AiResults",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "AiResults",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prompt",
                table: "AiResults",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AiResults",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
