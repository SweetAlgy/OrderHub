namespace OrderHub.Domain.Enums
{
    /// <summary>
    /// Represents the lifecycle status of an <see cref="OrderHub.Domain.Entities.Order"/>.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>The order has been placed but not yet dispatched.</summary>
        Created = 1,

        /// <summary>The order has been handed over to a carrier for delivery.</summary>
        Shipped = 2,

        /// <summary>The order has been successfully delivered to the client.</summary>
        Delivered = 3,

        /// <summary>The order has been cancelled and will not be fulfilled.</summary>
        Cancelled = 4
    }
}