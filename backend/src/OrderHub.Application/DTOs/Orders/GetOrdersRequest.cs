using System.ComponentModel.DataAnnotations;
using OrderHub.Domain.Enums;

namespace OrderHub.Application.DTOs.Orders
{
    /// <summary>
    /// Query parameters for the paginated orders endpoint. All filter fields are optional;
    /// omitting a field means no filtering is applied for that field.
    /// </summary>
    public record GetOrdersRequest
    {
        /// <summary>When set, returns only orders belonging to the specified client.</summary>
        public long? ClientId { get; init; }

        /// <summary>When set, returns only orders with the specified status.</summary>
        public OrderStatus? Status { get; init; }

        /// <summary>When set, performs a case-insensitive substring match on the order number.</summary>
        public string? OrderNumber { get; init; }

        /// <summary>When set, performs a case-insensitive substring match on the description.</summary>
        public string? Description { get; init; }

        /// <summary>
        /// When <c>true</c> (default), results are sorted in descending order;
        /// when <c>false</c>, in ascending order.
        /// </summary>
        public bool SortDescending { get; init; } = true;

        /// <summary>
        /// The field name to sort by. Supported values: <c>orderNumber</c>, <c>description</c>,
        /// <c>updatedAt</c>, <c>status</c>, <c>clientName</c>. Defaults to <c>createdAt</c>.
        /// </summary>
        public string? SortBy { get; init; }

        /// <summary>1-based page number to retrieve. Must be at least 1. Defaults to <c>1</c>.</summary>
        [Range(1, Int32.MaxValue)]
        public int Page { get; init; } = 1;

        /// <summary>Maximum number of orders per page. Must be between 1 and 100. Defaults to <c>20</c>.</summary>
        [Range(1, 100)]
        public int PageSize { get; init; } = 20;
    }
}