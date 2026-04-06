using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GlobalId_removed_and_SourceId_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "GlobalId",
                table: "ContactPersons");

            migrationBuilder.AddColumn<byte>(
                name: "SourceId",
                table: "Users",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SourceId",
                table: "Firms",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SourceId",
                table: "Events",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SourceId",
                table: "Deals",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SourceId",
                table: "ContactPersons",
                type: "smallint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "ContactPersons");

            migrationBuilder.AddColumn<Guid>(
                name: "GlobalId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GlobalId",
                table: "Firms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GlobalId",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GlobalId",
                table: "Deals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GlobalId",
                table: "ContactPersons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
