using Microsoft.Extensions.Logging;
using OrderHub.Application.DTOs.Clients;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Application.Interfaces.Services;
using OrderHub.Domain.Entities;
using OrderHub.Shared.Pagination;
using OrderHub.Shared.Results;

namespace OrderHub.Application.Services
{
    /// <summary>
    /// Implements <see cref="IClientService"/> using <see cref="IClientRepository"/>
    /// for persistence and structured logging via source-generated log methods.
    /// </summary>
    public sealed partial class ClientService(
        IClientRepository clientRepository,
        ILogger<ClientService> logger
    ) : IClientService
    {
        private readonly IClientRepository _clientRepository = clientRepository;
        private readonly ILogger<ClientService> _logger = logger;

        /// <inheritdoc/>
        public async Task<Result<ClientDto>> CreateAsync(
            CreateClientDto dto,
            CancellationToken cancellationToken = default
        )
        {
            this.LogCreatingClient(dto.Name);

            var client = new Client { Name = dto.Name };
            var result = await this._clientRepository.AddAsync(client, cancellationToken);

            if (result.IsFailure)
            {
                this.LogCreateClientFailed(dto.Name, result.Error.Code);
                return Result.Failure<ClientDto>(result.Error);
            }

            this.LogClientCreated(client.Id, client.Name);
            return Result.Success(ClientService.MapToDto(client));
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ClientDto>> GetPagedAsync(
            PageRequest pageRequest,
            CancellationToken cancellationToken = default
        )
        {
            this.LogGettingPagedClients(pageRequest.Page, pageRequest.PageSize);
            var clients = await this._clientRepository.GetPagedAsync(pageRequest, cancellationToken);
            this.LogPagedClientsLoaded(clients.TotalCount, clients.Page, clients.PageSize);
            return clients.To(clients.Items.Select(ClientService.MapToDto));
        }

        /// <summary>Maps a <see cref="Client"/> domain entity to a <see cref="ClientDto"/> read model.</summary>
        private static ClientDto MapToDto(Client client) => new() { Id = client.Id, Name = client.Name };

        [LoggerMessage(Level = LogLevel.Information, Message = "Creating client with name {ClientName}")]
        private partial void LogCreatingClient(string clientName);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to create client {ClientName}: {ErrorCode}")]
        private partial void LogCreateClientFailed(string clientName, string errorCode);

        [LoggerMessage(Level = LogLevel.Information, Message = "Client {ClientId} created with name {ClientName}")]
        private partial void LogClientCreated(long clientId, string clientName);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Getting paged clients: page {Page}, pageSize {PageSize}")]
        private partial void LogGettingPagedClients(int page, int pageSize);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Loaded {TotalCount} clients (page {Page}, pageSize {PageSize})")]
        private partial void LogPagedClientsLoaded(int totalCount, int page, int pageSize);
    }
}
