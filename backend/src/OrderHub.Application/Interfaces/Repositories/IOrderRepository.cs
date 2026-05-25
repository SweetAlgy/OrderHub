using OrderHub.Application.DTOs.Orders;
using OrderHub.Application.Filters;
using OrderHub.Domain.Entities;
using OrderHub.Domain.Enums;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines persistence operations for the <see cref="Order"/> aggregate.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Retrieves a single order by its primary key.
        /// </summary>
        /// <param name="id">The order's identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A success result containing the <see cref="Order"/>, or a 404 failure result
        /// if no order with the given <paramref name="id"/> exists.
        /// </returns>
        public Task<Result<Order>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a filtered and sorted page of orders, eagerly loading the associated client.
        /// </summary>
        /// <param name="pageRequest">Pagination parameters.</param>
        /// <param name="filter">Filter and sort criteria to apply.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A <see cref="PagedResult{T}"/> containing the matching orders on the requested page.</returns>
        public Task<PagedResult<Order>> GetPagedAsync(
            PageRequest pageRequest,
            OrderFilter filter,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Checks whether an order with the given <paramref name="orderNumber"/> already exists.
        /// </summary>
        /// <param name="orderNumber">The order number to look up.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><c>true</c> if the order number is already taken; otherwise <c>false</c>.</returns>
        public Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists a new <paramref name="order"/> to the data store.
        /// Returns a 409 conflict failure if the order number is already taken.
        /// </summary>
        /// <param name="order">The order entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A success result, or a failure result describing the error.</returns>
        public Task<Result> AddAsync(Order order, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the <see cref="Order.Status"/> and <see cref="Order.UpdatedAt"/> fields of
        /// the order identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier of the order to update.</param>
        /// <param name="status">The new status to apply.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A success result containing an <see cref="UpdateOrderStatusResult"/> with the updated fields,
        /// or a 404 failure result if no order with the given <paramref name="id"/> exists.
        /// </returns>
        public Task<Result<UpdateOrderStatusResult>> UpdateStatusAsync(
            Guid id,
            OrderStatus status,
            CancellationToken cancellationToken = default
        );
    }
}