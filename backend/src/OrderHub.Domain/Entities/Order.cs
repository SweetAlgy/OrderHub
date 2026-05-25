using OrderHub.Domain.Enums;
using OrderHub.Shared.Interfaces.Entities;

namespace OrderHub.Domain.Entities
{
    /// <summary>
    /// Represents a customer order in the system.
    /// </summary>
    public sealed class Order : ITrackableEntity<Guid>
    {
        /// <inheritdoc/>
        public Guid Id { get; init; }

        /// <inheritdoc/>
        public DateTime CreatedAt { get; init; }

        /// <inheritdoc/>
        public DateTime UpdatedAt { get; init; }

        /// <summary>Gets the identifier of the client who placed this order.</summary>
        public long ClientId { get; init; }

        /// <summary>
        /// Gets the navigation property to the associated <see cref="Entities.Client"/>.
        /// May be <c>null</c> if the client was not eagerly loaded.
        /// </summary>
        public Client? Client { get; init; }

        /// <summary>Gets the human-readable order number (e.g. <c>"ORD-0001"</c>). Must be unique.</summary>
        public string OrderNumber { get; init; } = String.Empty;

        /// <summary>Gets a free-text description of the order contents or instructions.</summary>
        public string Description { get; init; } = String.Empty;

        /// <summary>Gets the current processing status of the order.</summary>
        public OrderStatus Status { get; init; }
    }
}