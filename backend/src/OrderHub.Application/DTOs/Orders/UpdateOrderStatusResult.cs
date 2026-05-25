using OrderHub.Domain.Enums;

namespace OrderHub.Application.DTOs.Orders
{
    /// <summary>
    /// Read model returned after a successful order status update, containing the
    /// fields that changed as a result of the operation.
    /// </summary>
    public record UpdateOrderStatusResult
    {
        /// <summary>Gets the unique identifier of the updated order.</summary>
        public Guid Id { get; init; }

        /// <summary>Gets the new status that was applied to the order.</summary>
        public OrderStatus Status { get; init; }

        /// <summary>Gets the UTC date and time when the status was last updated.</summary>
        public DateTime UpdatedAt { get; init; }
    }
}