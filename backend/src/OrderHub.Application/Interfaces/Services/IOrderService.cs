using OrderHub.Application.DTOs.Orders;
using OrderHub.Application.Filters;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Application.Interfaces.Services
{
    /// <summary>
    /// Defines business operations for managing orders.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Retrieves a single order by its unique identifier.
        /// </summary>
        /// <param name="id">The order's identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A success result containing the <see cref="OrderDto"/>, or a 404 failure result
        /// if no order with the given <paramref name="id"/> exists.
        /// </returns>
        public Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a filtered and sorted page of orders.
        /// </summary>
        /// <param name="pageRequest">Pagination parameters.</param>
        /// <param name="filter">Filter and sort criteria.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A <see cref="PagedResult{T}"/> containing the matching orders on the requested page.</returns>
        public Task<PagedResult<OrderDto>> GetPagedAsync(
            PageRequest pageRequest,
            OrderFilter filter,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Creates a new order from the given <paramref name="dto"/>.
        /// Validates that the referenced client exists and that the order number is unique.
        /// </summary>
        /// <param name="dto">The data required to create the order.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A success result containing the created <see cref="OrderDto"/>,
        /// or a failure result (404 if the client is not found, 409 if the order number is taken).
        /// </returns>
        public Task<Result<OrderDto>> CreateAsync(
            CreateOrderDto dto,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Updates the status of an existing order and broadcasts a real-time notification
        /// to all subscribers of that order.
        /// </summary>
        /// <param name="id">The identifier of the order to update.</param>
        /// <param name="dto">The payload containing the new status.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A success result containing an <see cref="UpdateOrderStatusResult"/> with the updated fields,
        /// or a 404 failure result if the order does not exist.
        /// </returns>
        public Task<Result<UpdateOrderStatusResult>> UpdateStatusAsync(
            Guid id,
            UpdateOrderStatusDto dto,
            CancellationToken cancellationToken = default
        );
    }
}