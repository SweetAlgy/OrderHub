using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using OrderHub.Application.DTOs.Orders;
using OrderHub.Application.Filters;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Domain.Entities;
using OrderHub.Domain.Enums;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IOrderRepository"/>.
    /// Status updates are executed as bulk <c>ExecuteUpdateAsync</c> calls to avoid
    /// loading the full entity into memory. Duplicate order-number violations from
    /// PostgreSQL are caught and translated into <see cref="Error.AlreadyExists"/> results.
    /// </summary>
    public partial class OrderRepository(
        ApplicationDbContext context,
        ILogger<OrderRepository> logger
    ) : IOrderRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<OrderRepository> _logger = logger;

        /// <inheritdoc/>
        public async Task<Result<Order>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            this.LogQueryingOrderById(id);

            var order = await this._context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

            if (order is null)
            {
                this.LogOrderNotFoundInDb(id);
                return Result.Failure<Order>(Error.NotFound(nameof(Order), id));
            }

            return Result.Success(order);
        }

        /// <inheritdoc/>
        public async Task<Result> AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            this.LogInsertingOrder(order.Id, order.OrderNumber);
            try
            {
                await this._context.Orders.AddAsync(order, cancellationToken);
                await this._context.SaveChangesAsync(cancellationToken);
                this.LogOrderInserted(order.Id);
                return Result.Success();
            }
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException
                                                      {
                                                          SqlState: PostgresErrorCodes.UniqueViolation
                                                      })
            {
                this.LogOrderNumberConflict(order.OrderNumber);
                return Result.Failure(Error.AlreadyExists(nameof(Order), nameof(Order.OrderNumber), order.OrderNumber));
            }
        }

        /// <inheritdoc/>
        public async Task<PagedResult<Order>> GetPagedAsync(
            PageRequest pageRequest,
            OrderFilter filter,
            CancellationToken cancellationToken = default
        )
        {
            IQueryable<Order> query = this._context.Orders
                .AsNoTracking()
                .Include(order => order.Client!);

            if (filter.ClientId.HasValue)
                query = query.Where(order => order.ClientId == filter.ClientId.Value);

            if (filter.Status.HasValue)
                query = query.Where(order => order.Status == filter.Status.Value);

            if (!String.IsNullOrEmpty(filter.OrderNumber))
                query = query.Where(order => order.OrderNumber.ToLower().Contains(filter.OrderNumber.ToLower()));

            if (!String.IsNullOrEmpty(filter.Description))
                query = query.Where(order => order.Description.ToLower().Contains(filter.Description.ToLower()));

            var totalCount = await query.CountAsync(cancellationToken);

            var orderedQuery = filter.SortBy switch
            {
                "orderNumber" => filter.SortDescending
                    ? query.OrderByDescending(order => order.OrderNumber)
                    : query.OrderBy(order => order.OrderNumber),
                "description" => filter.SortDescending
                    ? query.OrderByDescending(order => order.Description)
                    : query.OrderBy(order => order.Description),
                "updatedAt" => filter.SortDescending
                    ? query.OrderByDescending(order => order.UpdatedAt)
                    : query.OrderBy(order => order.UpdatedAt),
                "status" => filter.SortDescending
                    ? query.OrderByDescending(order => order.Status)
                    : query.OrderBy(order => order.Status),
                "clientName" => filter.SortDescending
                    ? query.OrderByDescending(order => order.Client!.Name)
                    : query.OrderBy(order => order.Client!.Name),
                _ => filter.SortDescending
                    ? query.OrderByDescending(order => order.CreatedAt)
                    : query.OrderBy(order => order.CreatedAt),
            };

            var items = await orderedQuery
                .ThenBy(order => order.Id)
                .Skip(pageRequest.Skip)
                .Take(pageRequest.PageSize)
                .ToListAsync(cancellationToken);

            this.LogOrdersQueried(totalCount, pageRequest.Page, pageRequest.PageSize);

            return new()
            {
                Items = items,
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByOrderNumberAsync(
            string orderNumber,
            CancellationToken cancellationToken = default
        )
        {
            return await this._context.Orders
                .AsNoTracking()
                .AnyAsync(order => order.OrderNumber == orderNumber, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<Result<UpdateOrderStatusResult>> UpdateStatusAsync(
            Guid id,
            OrderStatus status,
            CancellationToken cancellationToken = default
        )
        {
            this.LogUpdatingOrderStatusInDb(id, status);

            var now = DateTime.UtcNow;

            var affected = await this._context.Orders
                .Where(order => order.Id == id)
                .ExecuteUpdateAsync(
                    property => property
                        .SetProperty(order => order.Status, status)
                        .SetProperty(order => order.UpdatedAt, now),
                    cancellationToken
                );

            if (affected == 0)
            {
                this.LogOrderNotFoundInDb(id);
                return Result.Failure<UpdateOrderStatusResult>(Error.NotFound(nameof(Order), id));
            }

            this.LogOrderStatusUpdatedInDb(id, status);
            return Result.Success(
                new UpdateOrderStatusResult()
                {
                    Id = id,
                    Status = status,
                    UpdatedAt = now
                }
            );
        }

        [LoggerMessage(Level = LogLevel.Debug, Message = "Querying order by id {OrderId}")]
        private partial void LogQueryingOrderById(Guid orderId);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Order {OrderId} not found in database")]
        private partial void LogOrderNotFoundInDb(Guid orderId);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Inserting order {OrderId} with number {OrderNumber}")]
        private partial void LogInsertingOrder(Guid orderId, string orderNumber);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Order {OrderId} inserted successfully")]
        private partial void LogOrderInserted(Guid orderId);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Order number {OrderNumber} already exists (unique violation)")]
        private partial void LogOrderNumberConflict(string orderNumber);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Orders query returned {TotalCount} records (page {Page}, pageSize {PageSize})")]
        private partial void LogOrdersQueried(int totalCount, int page, int pageSize);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Updating order {OrderId} status to {Status} in database")]
        private partial void LogUpdatingOrderStatusInDb(Guid orderId, OrderStatus status);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Order {OrderId} status updated to {Status} in database")]
        private partial void LogOrderStatusUpdatedInDb(Guid orderId, OrderStatus status);
    }
}
