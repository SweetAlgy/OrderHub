using OrderHub.Domain.Entities;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines persistence operations for the <see cref="Client"/> aggregate.
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>
        /// Persists a new <paramref name="client"/> to the data store.
        /// </summary>
        /// <param name="client">The client entity to add.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A success result, or a failure result if the operation could not be completed.</returns>
        public Task<Result> AddAsync(Client client, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a single page of clients ordered by name.
        /// </summary>
        /// <param name="pageRequest">Pagination parameters (page number and page size).</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A <see cref="PagedResult{T}"/> containing the clients on the requested page.</returns>
        public Task<PagedResult<Client>> GetPagedAsync(
            PageRequest pageRequest,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Retrieves a single client by its primary key.
        /// </summary>
        /// <param name="id">The client's identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A success result containing the <see cref="Client"/>, or a 404 failure result
        /// if no client with the given <paramref name="id"/> exists.
        /// </returns>
        public Task<Result<Client>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a client with the given <paramref name="id"/> exists in the data store.
        /// </summary>
        /// <param name="id">The client's identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns><c>true</c> if a client with the given ID exists; otherwise <c>false</c>.</returns>
        public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
    }
}