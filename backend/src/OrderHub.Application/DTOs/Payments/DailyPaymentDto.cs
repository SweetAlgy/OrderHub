namespace OrderHub.Application.DTOs.Payments
{
    /// <summary>
    /// Read model representing the total payments received from a client on a single day.
    /// Returned as an element of the daily payment summary list.
    /// </summary>
    public class DailyPaymentDto
    {
        /// <summary>Gets the date on which payments were received.</summary>
        public DateOnly PaymentDate { get; init; }

        /// <summary>Gets the aggregated payment amount for the day.</summary>
        public decimal Amount { get; init; }
    }
}