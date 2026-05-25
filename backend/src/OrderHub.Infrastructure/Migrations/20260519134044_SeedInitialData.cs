using Microsoft.EntityFrameworkCore.Migrations;
using OrderHub.Infrastructure.Seed;

#nullable disable

namespace OrderHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DataSeeder.Seed(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "client_payments", keyColumn: "id", keyValues: [1L, 2L, 3L, 4L, 5L, 6L, 7L, 8L, 9L]);
            migrationBuilder.DeleteData(table: "orders", keyColumn: "id", keyValues: [
                "a1b2c3d4-0001-0000-0000-000000000001",
                "a1b2c3d4-0002-0000-0000-000000000002",
                "a1b2c3d4-0003-0000-0000-000000000003",
                "a1b2c3d4-0004-0000-0000-000000000004",
                "a1b2c3d4-0005-0000-0000-000000000005"
            ]);
            migrationBuilder.DeleteData(table: "clients", keyColumn: "id", keyValues: [1L, 2L, 3L]);
        }
    }
}
