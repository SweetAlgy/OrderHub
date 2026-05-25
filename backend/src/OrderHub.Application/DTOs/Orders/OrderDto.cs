using OrderHub.Domain.Enums;

namespace OrderHub.Application.DTOs.Orders
{
    /// <summary>
    /// Read model returned by the API to represent a full order with client details.
    /// </summary>
    public record OrderDto
    {
        /// <summary>Gets the unique identifier of the order.</summary>
        public Guid Id { get; init; }

        /// <summary>Gets the identifier of the client who placed this order.</summary>
        public long ClientId { get; init; }

        /// <summary>Gets the display name of the client who placed this order.</summary>
        public string ClientName { get; init; } = String.Empty;

        /// <summary>Gets the human-readable order number. Unique across the system.</summary>
        public string OrderNumber { get; init; } = String.Empty;

        /// <summary>Gets the free-text description of the order.</summary>
        public string Description { get; init; } = String.Empty;

        /// <summary>Gets the current processing status of the order.</summary>
        public OrderStatus Status { get; init; }

        /// <summary>Gets the UTC date and time when the order was created.</summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>Gets the UTC date and time when the order was last updated.</summary>
        public DateTime UpdatedAt { get; init; }
    }
}