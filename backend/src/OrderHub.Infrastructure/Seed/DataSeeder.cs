using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderHub.Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static void Seed(MigrationBuilder migrationBuilder)
        {
            // Clients
            migrationBuilder.InsertData(
                table: "clients",
                columns: ["id", "name"],
                values: new object[,]
                {
                    { 1L, "Alice Johnson" },
                    { 2L, "Bob Smith" },
                    { 3L, "Carol White" }
                }
            );

            // Orders
            migrationBuilder.InsertData(
                table: "orders",
                columns: ["id", "client_id", "order_number", "description", "status", "created_at", "updated_at"],
                values: new object[,]
                {
                    {
                        Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001"), 1L, "ORD-001", "Laptop purchase", 1,
                        DateTime.Parse("2024-01-10T10:00:00Z").ToUniversalTime(),
                        DateTime.Parse("2024-01-10T10:00:00Z").ToUniversalTime()
                    },
                    {
                        Guid.Parse("a1b2c3d4-0002-0000-0000-000000000002"), 1L, "ORD-002", "Mouse and keyboard", 2,
                        DateTime.Parse("2024-01-15T12:00:00Z").ToUniversalTime(),
                        DateTime.Parse("2024-01-16T09:00:00Z").ToUniversalTime()
                    },
                    {
                        Guid.Parse("a1b2c3d4-0003-0000-0000-000000000003"), 2L, "ORD-003", "Office chair", 3,
                        DateTime.Parse("2024-01-20T08:00:00Z").ToUniversalTime(),
                        DateTime.Parse("2024-01-25T14:00:00Z").ToUniversalTime()
                    },
                    {
                        Guid.Parse("a1b2c3d4-0004-0000-0000-000000000004"), 2L, "ORD-004", "Standing desk", 4,
                        DateTime.Parse("2024-02-01T11:00:00Z").ToUniversalTime(),
                        DateTime.Parse("2024-02-02T10:00:00Z").ToUniversalTime()
                    },
                    {
                        Guid.Parse("a1b2c3d4-0005-0000-0000-000000000005"), 3L, "ORD-005", "Monitor 27 inch", 1,
                        DateTime.Parse("2024-02-10T09:00:00Z").ToUniversalTime(),
                        DateTime.Parse("2024-02-10T09:00:00Z").ToUniversalTime()
                    }
                }
            );
            
            // ClientPayments
            migrationBuilder.InsertData(
                table: "client_payments",
                columns: ["id", "client_id", "payment_date", "amount"],
                values: new object[,]
                {
                    { 1L, 1L, DateTime.Parse("2022-01-03T17:24:00Z").ToUniversalTime(), 100m },
                    { 2L, 1L, DateTime.Parse("2022-01-05T17:24:14Z").ToUniversalTime(), 200m },
                    { 3L, 1L, DateTime.Parse("2022-01-05T18:23:34Z").ToUniversalTime(), 250m },
                    { 4L, 1L, DateTime.Parse("2022-01-07T10:12:38Z").ToUniversalTime(), 50m  },
                    { 5L, 2L, DateTime.Parse("2022-01-05T17:24:14Z").ToUniversalTime(), 278m },
                    { 6L, 2L, DateTime.Parse("2022-01-10T12:39:29Z").ToUniversalTime(), 300m },
                    { 7L, 3L, DateTime.Parse("2024-01-10T09:00:00Z").ToUniversalTime(), 500m },
                    { 8L, 3L, DateTime.Parse("2024-01-10T15:00:00Z").ToUniversalTime(), 150m },
                    { 9L, 3L, DateTime.Parse("2024-01-12T11:00:00Z").ToUniversalTime(), 320m }
                }
            );
        }
    }
}