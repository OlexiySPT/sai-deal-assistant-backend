using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Sai.DealAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SampleCustomerAndEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SampleCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "varchar", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampleEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, computedColumnSql: "\"FirstName\" || ' ' || \"LastName\"", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SampleEmployees_SampleCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "SampleCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleCustomers_Code",
                table: "SampleCustomers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleEmployees_CustomerId",
                table: "SampleEmployees",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleEmployees_Email",
                table: "SampleEmployees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleEmployees_FullName",
                table: "SampleEmployees",
                column: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleEmployees");

            migrationBuilder.DropTable(
                name: "SampleCustomers");
        }
    }
}
