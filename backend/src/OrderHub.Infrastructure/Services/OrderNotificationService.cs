using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OrderHub.Application.Interfaces.Services;
using OrderHub.Domain.Enums;
using OrderHub.Infrastructure.Hubs;

namespace OrderHub.Infrastructure.Services
{
    /// <summary>
    /// SignalR-based implementation of <see cref="IOrderNotificationService"/>.
    /// Broadcasts the <c>OrderStatusChanged</c> event to all members of the order's
    /// SignalR group via <see cref="OrderStatusHub"/>.
    /// </summary>
    public sealed partial class OrderNotificationService(
        IHubContext<OrderStatusHub> hubContext,
        ILogger<OrderNotificationService> logger
    ) : IOrderNotificationService
    {
        private readonly IHubContext<OrderStatusHub> _hubContext = hubContext;
        private readonly ILogger<OrderNotificationService> _logger = logger;

        /// <inheritdoc/>
        public async Task NotifyStatusChangedAsync(
            Guid orderId,
            OrderStatus newStatus,
            DateTime updatedAt,
            CancellationToken cancellationToken = default
        )
        {
            this.LogSendingStatusNotification(orderId, newStatus);

            await this._hubContext.Clients
                .Group(orderId.ToString())
                .SendAsync(
                    "OrderStatusChanged",
                    new
                    {
                        orderId, status = newStatus.ToString(),
                        updatedAt = updatedAt.ToString(CultureInfo.InvariantCulture)
                    },
                    cancellationToken
                );

            this.LogStatusNotificationSent(orderId, newStatus);
        }

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Sending SignalR status notification for order {OrderId}: {Status}"
        )]
        private partial void LogSendingStatusNotification(Guid orderId, OrderStatus status);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "SignalR notification sent for order {OrderId}: status changed to {Status}"
        )]
        private partial void LogStatusNotificationSent(Guid orderId, OrderStatus status);
    }
}
