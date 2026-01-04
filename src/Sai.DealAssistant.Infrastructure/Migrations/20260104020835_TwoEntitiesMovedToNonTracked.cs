using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TwoEntitiesMovedToNonTracked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EventNotes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "EventNotes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EventNotes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "EventNotes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DealTags");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DealTags");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DealTags");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "DealTags");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "DealTags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "EventNotes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "EventNotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "EventNotes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "EventNotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "DealTags",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "DealTags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "DealTags",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "DealTags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "DealTags",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }
    }
}
