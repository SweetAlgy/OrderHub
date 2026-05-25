using Microsoft.Extensions.Logging;
using OrderHub.Application.DTOs.Orders;
using OrderHub.Application.Filters;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Application.Interfaces.Services;
using OrderHub.Domain.Entities;
using OrderHub.Domain.Enums;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Application.Services
{
    /// <summary>
    /// Implements <see cref="IOrderService"/> using repository and notification service abstractions.
    /// After a successful status update the service pushes a real-time notification via
    /// <see cref="IOrderNotificationService"/>.
    /// </summary>
    public sealed partial class OrderService(
        IOrderRepository orderRepository,
        IClientRepository clientRepository,
        IOrderNotificationService orderNotificationService,
        ILogger<OrderService> logger
    ) : IOrderService
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IClientRepository _clientRepository = clientRepository;
        private readonly IOrderNotificationService _orderNotificationService = orderNotificationService;
        private readonly ILogger<OrderService> _logger = logger;

        /// <inheritdoc/>
        public async Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            this.LogGettingOrderById(id);
            var result = await this._orderRepository.GetByIdAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                this.LogOrderNotFound(id);
                return Result.Failure<OrderDto>(result.Error);
            }
            return Result.Success(OrderService.MapToDto(result.Value));
        }

        /// <inheritdoc/>
        public async Task<PagedResult<OrderDto>> GetPagedAsync(
            PageRequest pageRequest,
            OrderFilter filter,
            CancellationToken cancellationToken = default
        )
        {
            this.LogGettingPagedOrders(pageRequest.Page, pageRequest.PageSize);
            var page = await this._orderRepository.GetPagedAsync(pageRequest, filter, cancellationToken);
            this.LogPagedOrdersLoaded(page.TotalCount, page.Page, page.PageSize);
            return page.To(page.Items.Select(OrderService.MapToDto));
        }

        /// <inheritdoc/>
        public async Task<Result<OrderDto>> CreateAsync(
            CreateOrderDto dto,
            CancellationToken cancellationToken = default
        )
        {
            this.LogCreatingOrder(dto.OrderNumber, dto.ClientId);

            var clientExists = await this._clientRepository.ExistsAsync(dto.ClientId, cancellationToken);
            if (!clientExists)
            {
                this.LogClientNotFoundForOrder(dto.ClientId);
                return Result.Failure<OrderDto>(Error.NotFound(nameof(Client), dto.ClientId));
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                ClientId = dto.ClientId,
                OrderNumber = dto.OrderNumber,
                Description = dto.Description,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await this._orderRepository.AddAsync(order, cancellationToken);
            if (result.IsFailure)
            {
                this.LogCreateOrderFailed(dto.OrderNumber, result.Error.Code);
                return Result.Failure<OrderDto>(result.Error);
            }

            this.LogOrderCreated(order.Id, order.OrderNumber);
            return Result.Success(OrderService.MapToDto(order));
        }

        /// <inheritdoc/>
        public async Task<Result<UpdateOrderStatusResult>> UpdateStatusAsync(
            Guid id,
            UpdateOrderStatusDto dto,
            CancellationToken cancellationToken = default
        )
        {
            this.LogUpdatingOrderStatus(id, dto.Status);

            var result = await this._orderRepository.UpdateStatusAsync(id, dto.Status, cancellationToken);
            if (result.IsFailure)
            {
                this.LogUpdateOrderStatusFailed(id, dto.Status, result.Error.Code);
                return result;
            }

            this.LogOrderStatusUpdated(id, dto.Status);

            await this._orderNotificationService.NotifyStatusChangedAsync(
                id,
                dto.Status,
                result.Value.UpdatedAt,
                cancellationToken
            );

            return result;
        }

        /// <summary>Maps an <see cref="Order"/> domain entity to an <see cref="OrderDto"/> read model.</summary>
        private static OrderDto MapToDto(Order order) => new()
        {
            Id = order.Id,
            ClientId = order.ClientId,
            OrderNumber = order.OrderNumber,
            Description = order.Description,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            ClientName = order.Client?.Name ?? String.Empty
        };

        [LoggerMessage(Level = LogLevel.Debug, Message = "Getting order by id {OrderId}")]
        private partial void LogGettingOrderById(Guid orderId);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Order {OrderId} not found")]
        private partial void LogOrderNotFound(Guid orderId);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Getting paged orders: page {Page}, pageSize {PageSize}")]
        private partial void LogGettingPagedOrders(int page, int pageSize);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Loaded {TotalCount} orders (page {Page}, pageSize {PageSize})")]
        private partial void LogPagedOrdersLoaded(int totalCount, int page, int pageSize);

        [LoggerMessage(Level = LogLevel.Information, Message = "Creating order {OrderNumber} for client {ClientId}")]
        private partial void LogCreatingOrder(string orderNumber, long clientId);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Client {ClientId} not found — order creation aborted")]
        private partial void LogClientNotFoundForOrder(long clientId);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to create order {OrderNumber}: {ErrorCode}")]
        private partial void LogCreateOrderFailed(string orderNumber, string errorCode);

        [LoggerMessage(Level = LogLevel.Information, Message = "Order {OrderId} created with number {OrderNumber}")]
        private partial void LogOrderCreated(Guid orderId, string orderNumber);

        [LoggerMessage(Level = LogLevel.Information, Message = "Updating order {OrderId} status to {Status}")]
        private partial void LogUpdatingOrderStatus(Guid orderId, OrderStatus status);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to update order {OrderId} status to {Status}: {ErrorCode}")]
        private partial void LogUpdateOrderStatusFailed(Guid orderId, OrderStatus status, string errorCode);

        [LoggerMessage(Level = LogLevel.Information, Message = "Order {OrderId} status updated to {Status}")]
        private partial void LogOrderStatusUpdated(Guid orderId, OrderStatus status);
    }
}
