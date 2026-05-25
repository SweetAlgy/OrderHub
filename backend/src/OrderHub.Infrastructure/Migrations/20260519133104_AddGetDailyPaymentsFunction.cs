using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetDailyPaymentsFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    CREATE OR REPLACE FUNCTION get_daily_payments(
                        p_client_id BIGINT,
                        p_start_date DATE,
                        p_end_date DATE
                    )
                    RETURNS TABLE(payment_date DATE, amount NUMERIC(18, 2))
                    LANGUAGE sql
                    AS $$
                        SELECT
                            d.payment_date::DATE,
                            COALESCE(SUM(cp.amount), 0) AS amount
                        FROM generate_series(p_start_date, p_end_date, INTERVAL '1 day') AS d(payment_date)
                        LEFT JOIN client_payments cp
                            ON cp.payment_date::DATE = d.payment_date::DATE
                            AND cp.client_id = p_client_id
                        GROUP BY d.payment_date
                        ORDER BY d.payment_date;
                    $$;
                "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_daily_payments;");
        }
    }
}
