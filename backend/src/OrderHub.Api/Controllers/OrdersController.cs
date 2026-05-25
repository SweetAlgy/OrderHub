using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderHub.Application.DTOs.Orders;
using OrderHub.Application.Filters;
using OrderHub.Application.Interfaces.Services;
using OrderHub.Shared.Pagination;

namespace OrderHub.Api.Controllers
{
    /// <summary>
    /// Handles HTTP requests for creating, querying, and updating orders.
    /// Base route: <c>api/orders</c>.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public sealed partial class OrdersController(
        IOrderService orderService,
        ILogger<OrdersController> logger
    ) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ILogger<OrdersController> _logger = logger;

        /// <summary>Returns a single order by its unique identifier.</summary>
        /// <param name="id">The order's GUID.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>200 OK with the <see cref="OrderDto"/>; 404 Not Found if it does not exist.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            this.LogGetOrderById(id);
            var result = await this._orderService.GetByIdAsync(id, cancellationToken);
            if (result.IsFailure)
            {
                this.LogGetOrderByIdNotFound(id);
                return this.NotFound(result.Error);
            }
            return this.Ok(result.Value);
        }

        /// <summary>
        /// Returns a paginated list of orders with optional filtering and sorting.
        /// </summary>
        /// <param name="request">Filter, sort, and pagination parameters.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>200 OK with a <see cref="PagedResult{OrderDto}"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] GetOrdersRequest request,
            CancellationToken cancellationToken = default
        )
        {
            this.LogGetPagedOrders(request.Page, request.PageSize);

            var filter = new OrderFilter
            {
                ClientId = request.ClientId,
                Status = request.Status,
                OrderNumber = request.OrderNumber,
                Description = request.Description,
                SortBy = request.SortBy,
                SortDescending = request.SortDescending
            };

            var cursorRequest = new PageRequest
            {
                Page = request.Page,
                PageSize = request.PageSize
            };

            var result = await this._orderService.GetPagedAsync(cursorRequest, filter, cancellationToken);
            return this.Ok(result);
        }

        /// <summary>Creates a new order for the specified client.</summary>
        /// <param name="dto">The order creation payload.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// 201 Created with the <see cref="OrderDto"/>;
        /// 400 Bad Request for invalid input;
        /// 404 Not Found if the referenced client does not exist;
        /// 409 Conflict if the order number is already taken.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateOrderDto dto,
            CancellationToken cancellationToken = default)
        {
            this.LogCreateOrder(dto.OrderNumber, dto.ClientId);

            var result = await this._orderService.CreateAsync(dto, cancellationToken);

            if (result.IsFailure)
            {
                this.LogCreateOrderFailed(dto.OrderNumber, result.Error.Code);
                return result.Error.Code switch
                {
                    var code when code.EndsWith("NotFound") => this.NotFound(result.Error),
                    var code when code.EndsWith("AlreadyExists") => this.Conflict(result.Error),
                    _ => this.BadRequest(result.Error)
                };
            }

            this.LogCreateOrderSucceeded(result.Value.Id, dto.OrderNumber);
            return this.CreatedAtAction(nameof(OrdersController.GetById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Updates the status of an existing order and triggers a real-time SignalR notification
        /// to all subscribers of that order.
        /// </summary>
        /// <param name="id">The identifier of the order to update.</param>
        /// <param name="dto">The payload containing the new status.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// 200 OK with an <see cref="UpdateOrderStatusResult"/>;
        /// 400 Bad Request for invalid input;
        /// 404 Not Found if the order does not exist.
        /// </returns>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromBody] UpdateOrderStatusDto dto,
            CancellationToken cancellationToken = default)
        {
            this.LogUpdateOrderStatus(id, dto.Status.ToString());

            var result = await this._orderService.UpdateStatusAsync(id, dto, cancellationToken);

            if (result.IsFailure)
            {
                this.LogUpdateOrderStatusFailed(id, result.Error.Code);
                return result.Error.Code switch
                {
                    var code when code.EndsWith("NotFound") => this.NotFound(result.Error),
                    _ => this.BadRequest(result.Error)
                };
            }

            this.LogUpdateOrderStatusSucceeded(id);
            return this.Ok(result.Value);
        }

        [LoggerMessage(Level = LogLevel.Debug, Message = "GET /orders/{OrderId}")]
        private partial void LogGetOrderById(Guid orderId);

        [LoggerMessage(Level = LogLevel.Warning, Message = "GET /orders/{OrderId} — not found")]
        private partial void LogGetOrderByIdNotFound(Guid orderId);

        [LoggerMessage(Level = LogLevel.Debug, Message = "GET /orders — page {Page}, pageSize {PageSize}")]
        private partial void LogGetPagedOrders(int page, int pageSize);

        [LoggerMessage(Level = LogLevel.Information, Message = "POST /orders — creating order {OrderNumber} for client {ClientId}")]
        private partial void LogCreateOrder(string orderNumber, long clientId);

        [LoggerMessage(Level = LogLevel.Warning, Message = "POST /orders — failed to create order {OrderNumber}: {ErrorCode}")]
        private partial void LogCreateOrderFailed(string orderNumber, string errorCode);

        [LoggerMessage(Level = LogLevel.Information, Message = "POST /orders — order {OrderId} ({OrderNumber}) created")]
        private partial void LogCreateOrderSucceeded(Guid orderId, string orderNumber);

        [LoggerMessage(Level = LogLevel.Information, Message = "PATCH /orders/{OrderId}/status — updating to {Status}")]
        private partial void LogUpdateOrderStatus(Guid orderId, string status);

        [LoggerMessage(Level = LogLevel.Warning, Message = "PATCH /orders/{OrderId}/status — failed: {ErrorCode}")]
        private partial void LogUpdateOrderStatusFailed(Guid orderId, string errorCode);

        [LoggerMessage(Level = LogLevel.Information, Message = "PATCH /orders/{OrderId}/status — updated successfully")]
        private partial void LogUpdateOrderStatusSucceeded(Guid orderId);
    }
}
