using OrderHub.Application.DTOs.Clients;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Application.Interfaces.Services
{
    /// <summary>
    /// Defines business operations for managing clients.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Creates a new client from the given <paramref name="dto"/>.
        /// </summary>
        /// <param name="dto">The data required to create the client.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A success result containing the newly created <see cref="ClientDto"/>,
        /// or a failure result if creation failed.
        /// </returns>
        public Task<Result<ClientDto>> CreateAsync(CreateClientDto dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a paginated list of all clients, ordered by name.
        /// </summary>
        /// <param name="pageRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A <see cref="PagedResult{T}"/> containing the clients on the requested page.</returns>
        public Task<PagedResult<ClientDto>> GetPagedAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);
    }
}