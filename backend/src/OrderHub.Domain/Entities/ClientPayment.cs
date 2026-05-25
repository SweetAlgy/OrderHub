using OrderHub.Shared.Entities;

namespace OrderHub.Domain.Entities
{
    /// <summary>
    /// Represents a single payment made by a client on a specific date.
    /// </summary>
    public sealed class ClientPayment : IEntity<long>
    {
        /// <inheritdoc/>
        public long Id { get; init; }

        /// <summary>Gets the identifier of the client who made this payment.</summary>
        public long ClientId { get; init; }

        /// <summary>Gets the date on which the payment was recorded.</summary>
        public DateTime PaymentDate { get; init; }

        /// <summary>Gets the payment amount with up to two decimal places.</summary>
        public decimal Amount { get; init; }
    }
}