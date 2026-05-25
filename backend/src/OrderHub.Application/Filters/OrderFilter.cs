using OrderHub.Domain.Enums;

namespace OrderHub.Application.Filters
{
    /// <summary>
    /// Carries the filter and sort criteria applied when querying the orders collection.
    /// All filter properties are optional; a <c>null</c> value means no restriction is applied.
    /// </summary>
    public record struct OrderFilter
    {
        /// <summary>When set, restricts results to orders belonging to the specified client.</summary>
        public long? ClientId { get; init; }

        /// <summary>When set, restricts results to orders with the specified status.</summary>
        public OrderStatus? Status { get; init; }

        /// <summary>When set, restricts results to orders whose description contains this substring (case-insensitive).</summary>
        public string? Description { get; init; }

        /// <summary>
        /// The field name to sort by. Supported values: <c>orderNumber</c>, <c>description</c>,
        /// <c>updatedAt</c>, <c>status</c>, <c>clientName</c>. Defaults to <c>createdAt</c> when <c>null</c>.
        /// </summary>
        public string? SortBy { get; init; }

        /// <summary>When set, restricts results to orders whose order number contains this substring (case-insensitive).</summary>
        public string? OrderNumber { get; init; }

        /// <summary>
        /// When <c>true</c>, sorting is applied in descending order; when <c>false</c>, in ascending order.
        /// </summary>
        public bool SortDescending { get; init; }
    }
}