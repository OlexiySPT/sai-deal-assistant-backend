using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AiPrompt_table_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PreviousValue",
                table: "DealStatusAudits",
                type: "varchar",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PreviousText",
                table: "DealStateIdAudits",
                type: "varchar",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "AiPrompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "varchar", maxLength: 255, nullable: false),
                    Version = table.Column<string>(type: "varchar", maxLength: 255, nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiPrompts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DealStatusAudits_DealId",
                table: "DealStatusAudits",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DealStateIdAudits_DealId",
                table: "DealStateIdAudits",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_AiPrompts_Key_Version",
                table: "AiPrompts",
                columns: new[] { "Key", "Version" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DealStateIdAudits_Deals_DealId",
                table: "DealStateIdAudits",
                column: "DealId",
                principalTable: "Deals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DealStatusAudits_Deals_DealId",
                table: "DealStatusAudits",
                column: "DealId",
                principalTable: "Deals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DealStateIdAudits_Deals_DealId",
                table: "DealStateIdAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_DealStatusAudits_Deals_DealId",
                table: "DealStatusAudits");

            migrationBuilder.DropTable(
                name: "AiPrompts");

            migrationBuilder.DropIndex(
                name: "IX_DealStatusAudits_DealId",
                table: "DealStatusAudits");

            migrationBuilder.DropIndex(
                name: "IX_DealStateIdAudits_DealId",
                table: "DealStateIdAudits");

            migrationBuilder.AlterColumn<string>(
                name: "PreviousValue",
                table: "DealStatusAudits",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PreviousText",
                table: "DealStateIdAudits",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
