using System.ComponentModel.DataAnnotations;
using OrderHub.Domain.Enums;

namespace OrderHub.Application.DTOs.Orders
{
    /// <summary>
    /// Payload for updating the status of an existing order.
    /// </summary>
    public class UpdateOrderStatusDto
    {
        /// <summary>Gets the new status to apply to the order.</summary>
        [Required]
        public required OrderStatus Status { get; init; }
    }
}