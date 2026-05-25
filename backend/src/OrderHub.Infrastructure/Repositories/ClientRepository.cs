using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Domain.Entities;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IClientRepository"/>.
    /// All reads are performed as no-tracking queries for improved performance.
    /// </summary>
    public sealed partial class ClientRepository(
        ApplicationDbContext context,
        ILogger<ClientRepository> logger
    ) : IClientRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ClientRepository> _logger = logger;

        /// <inheritdoc/>
        public async Task<Result> AddAsync(Client client, CancellationToken cancellationToken = default)
        {
            this.LogInsertingClient(client.Name);
            await this._context.Clients.AddAsync(client, cancellationToken);
            await this._context.SaveChangesAsync(cancellationToken);
            this.LogClientInserted(client.Id, client.Name);
            return Result.Success();
        }

        /// <inheritdoc/>
        public async Task<PagedResult<Client>> GetPagedAsync(
            PageRequest pageRequest,
            CancellationToken cancellationToken = default
        )
        {
            var query = this._context.Clients
                .AsNoTracking()
                .OrderBy(client => client.Name);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(client => client.Name)
                .ThenBy(client => client.Id)
                .Skip(pageRequest.Skip)
                .Take(pageRequest.PageSize)
                .ToListAsync(cancellationToken);

            this.LogClientsQueried(totalCount, pageRequest.Page, pageRequest.PageSize);

            return new()
            {
                Items = items,
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<Result<Client>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            this.LogQueryingClientById(id);

            var client = await this._context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(client => client.Id == id, cancellationToken);

            if (client is null)
            {
                this.LogClientNotFoundInDb(id);
                return Result.Failure<Client>(Error.NotFound(nameof(Client), id));
            }

            return Result.Success(client);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
        {
            return await this._context.Clients
                .AsNoTracking()
                .AnyAsync(client => client.Id == id, cancellationToken);
        }

        [LoggerMessage(Level = LogLevel.Debug, Message = "Inserting client with name {ClientName}")]
        private partial void LogInsertingClient(string clientName);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Client {ClientId} ({ClientName}) inserted successfully")]
        private partial void LogClientInserted(long clientId, string clientName);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Clients query returned {TotalCount} records (page {Page}, pageSize {PageSize})")]
        private partial void LogClientsQueried(int totalCount, int page, int pageSize);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Querying client by id {ClientId}")]
        private partial void LogQueryingClientById(long clientId);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Client {ClientId} not found in database")]
        private partial void LogClientNotFoundInDb(long clientId);
    }
}
