using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace OrderHub.Infrastructure.Hubs
{
    /// <summary>
    /// SignalR hub that allows clients to subscribe and unsubscribe from real-time
    /// order status updates. Each order is represented as a separate SignalR group
    /// identified by the order's string ID.
    /// </summary>
    public sealed partial class OrderStatusHub(ILogger<OrderStatusHub> logger) : Hub
    {
        private readonly ILogger<OrderStatusHub> _logger = logger;

        /// <summary>
        /// Adds the calling connection to the SignalR group for the specified order,
        /// so it receives future status-change notifications for that order.
        /// </summary>
        /// <param name="orderId">The string representation of the order's GUID.</param>
        public async Task SubscribeToOrder(string orderId)
        {
            try
            {
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, orderId);
                this.LogSubscribed(this.Context.ConnectionId, orderId);
            }
            catch (Exception exception)
            {
                this.LogSubscribeError(exception, orderId);
                throw;
            }
        }

        /// <summary>
        /// Removes the calling connection from the SignalR group for the specified order.
        /// </summary>
        /// <param name="orderId">The string representation of the order's GUID.</param>
        public async Task UnsubscribeToOrder(string orderId)
        {
            try
            {
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, orderId);
                this.LogUnsubscribed(this.Context.ConnectionId, orderId);
            }
            catch (Exception exception)
            {
                this.LogUnsubscribeError(exception, orderId);
                throw;
            }
        }

        /// <summary>
        /// Adds the calling connection to multiple order groups simultaneously.
        /// All subscriptions are started in parallel using <see cref="Task.WhenAll"/>.
        /// </summary>
        /// <param name="orderIds">An array of order GUID strings to subscribe to.</param>
        public async Task SubscribeToOrders(string[] orderIds)
        {
            try
            {
                await Task.WhenAll(
                    orderIds.Select(orderId => 
                        this.Groups.AddToGroupAsync(this.Context.ConnectionId, orderId))
                );
                
                this.LogOrdersSubscribed(this.Context.ConnectionId, orderIds.Length);
            }
            catch (Exception exception)
            {
                this.LogOrdersSubscribeError(exception, orderIds.Length);
                throw;
            }
        }

        /// <summary>
        /// Removes the calling connection from multiple order groups simultaneously.
        /// All removals are performed in parallel using <see cref="Task.WhenAll"/>.
        /// </summary>
        /// <param name="orderIds">An array of order GUID strings to unsubscribe from.</param>
        public async Task UnsubscribeFromOrders(string[] orderIds)
        {
            try
            {
                await Task.WhenAll(
                    orderIds.Select(orderId => 
                        this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, orderId))
                );
                
                this.LogOrdersUnsubscribed(this.Context.ConnectionId, orderIds.Length);
            }
            catch (Exception exception)
            {
                this.LogOrdersUnsubscribeError(exception, orderIds.Length);
                throw;
            }
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Client {ConnectionId} subscribed to order {OrderId}")]
        private partial void LogSubscribed(string connectionId, string orderId);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Client {ConnectionId} unsubscribed from order {OrderId}"
        )]
        private partial void LogUnsubscribed(string connectionId, string orderId);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to subscribe to order {OrderId}")]
        private partial void LogSubscribeError(Exception exception, string orderId);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to unsubscribe from order {OrderId}")]
        private partial void LogUnsubscribeError(Exception exception, string orderId);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Client {ConnectionId} subscribed to {OrdersCount} orders"
        )]
        private partial void LogOrdersSubscribed(string connectionId, int ordersCount);

        [LoggerMessage(LogLevel.Information, Message = "Client {ConnectionId} unsubscribed from {OrdersCount} orders")]
        private partial void LogOrdersUnsubscribed(string connectionId, int ordersCount);

        [LoggerMessage(LogLevel.Error, Message = "Failed to subscribe to {OrdersCount} orders")]
        private partial void LogOrdersSubscribeError(Exception exception, int ordersCount);

        [LoggerMessage(LogLevel.Error, Message = "Failed to unsubscribe from {OrdersCount} orders")]
        private partial void LogOrdersUnsubscribeError(Exception exception, int ordersCount);
    }
}