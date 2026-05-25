using System.ComponentModel.DataAnnotations;
using OrderHub.Shared.Constants;

namespace OrderHub.Application.DTOs.Orders
{
    /// <summary>
    /// Payload for creating a new order.
    /// </summary>
    public record CreateOrderDto
    {
        /// <summary>Gets the identifier of the client for whom the order is created.</summary>
        [Required]
        public required long ClientId { get; init; }

        /// <summary>
        /// Gets the human-readable order number. Must be unique across the system
        /// and must not exceed <see cref="OrderConstants.OrderNumberMaxLength"/> characters.
        /// </summary>
        [Required]
        [MaxLength(OrderConstants.OrderNumberMaxLength)]
        public required string OrderNumber { get; init; }

        /// <summary>
        /// Gets the free-text description of the order.
        /// Must not exceed <see cref="OrderConstants.DescriptionMaxLength"/> characters.
        /// </summary>
        [Required]
        [MaxLength(OrderConstants.DescriptionMaxLength)]
        public required string Description { get; init; }
    }
}