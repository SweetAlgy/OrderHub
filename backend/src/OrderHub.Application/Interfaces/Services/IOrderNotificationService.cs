using OrderHub.Domain.Enums;

namespace OrderHub.Application.Interfaces.Services
{
    /// <summary>
    /// Defines a contract for broadcasting real-time order lifecycle events to connected clients.
    /// </summary>
    public interface IOrderNotificationService
    {
        /// <summary>
        /// Sends a status-change notification to all SignalR clients subscribed to the given order.
        /// </summary>
        /// <param name="orderId">The identifier of the order whose status changed.</param>
        /// <param name="newStatus">The new status of the order.</param>
        /// <param name="updatedAt">The UTC timestamp of the status change.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        public Task NotifyStatusChangedAsync(
            Guid orderId,
            OrderStatus newStatus,
            DateTime updatedAt,
            CancellationToken cancellationToken = default
        );
    }
}