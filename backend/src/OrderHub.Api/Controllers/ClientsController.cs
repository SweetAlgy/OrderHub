using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderHub.Application.DTOs.Clients;
using OrderHub.Application.DTOs.Orders;
using OrderHub.Application.DTOs.Payments;
using OrderHub.Application.Interfaces.Services;
using OrderHub.Shared.Pagination;

namespace OrderHub.Api.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to clients and their payment summaries.
    /// Base route: <c>api/clients</c>.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public sealed partial class ClientsController(
        IClientService clientService,
        IClientPaymentService clientPaymentService,
        ILogger<ClientsController> logger
    ) : ControllerBase
    {
        private readonly IClientService _clientService = clientService;
        private readonly IClientPaymentService _clientPaymentService = clientPaymentService;
        private readonly ILogger<ClientsController> _logger = logger;

        /// <summary>Returns a paginated list of all clients ordered by name.</summary>
        /// <param name="pageRequest">Pagination parameters.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>200 OK with a <see cref="PagedResult{ClientDto}"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ClientDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] PageRequest pageRequest,
            CancellationToken cancellationToken = default
        )
        {
            this.LogGetPagedClients(pageRequest.Page, pageRequest.PageSize);
            var clients = await this._clientService.GetPagedAsync(pageRequest, cancellationToken);
            return this.Ok(clients);
        }

        /// <summary>Creates a new client.</summary>
        /// <param name="dto">The client creation payload.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// 201 Created with the newly created <see cref="ClientDto"/>;
        /// 409 Conflict if a client with the same name already exists.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateClientDto dto,
            CancellationToken cancellationToken = default)
        {
            this.LogCreateClient(dto.Name);

            var result = await this._clientService.CreateAsync(dto, cancellationToken);
            if (result.IsFailure)
            {
                this.LogCreateClientFailed(dto.Name, result.Error.Code);
                return this.StatusCode(result.Error.StatusCode, result.Error);
            }

            this.LogCreateClientSucceeded(result.Value.Id, dto.Name);
            return this.CreatedAtAction(nameof(ClientsController.GetPaged), result.Value);
        }

        /// <summary>
        /// Returns the daily payment totals for a specific client within the given date range.
        /// </summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="request">Date-range query parameters.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// 200 OK with a list of <see cref="DailyPaymentDto"/> records;
        /// 400 Bad Request if the date range is invalid;
        /// 404 Not Found if the client does not exist.
        /// </returns>
        [HttpGet("{clientId:long}/payments/daily-summary")]
        [ProducesResponseType(typeof(IEnumerable<DailyPaymentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDailySummary(
            long clientId,
            [FromQuery] GetDailyPaymentsRequest request,
            CancellationToken cancellationToken = default
        )
        {
            this.LogGetDailySummary(clientId, request.From, request.To);

            var validationErrors = request.Validate().ToList();
            if (validationErrors.Count != 0)
            {
                this.LogGetDailySummaryValidationFailed(clientId, validationErrors.Count);
                return this.BadRequest(validationErrors.Select(e => e.ErrorMessage));
            }

            var result = await this._clientPaymentService.GetDailyPaymentsAsync(
                clientId,
                request.From,
                request.To,
                cancellationToken
            );

            if (result.IsFailure)
            {
                this.LogGetDailySummaryFailed(clientId, result.Error.Code);
                return this.StatusCode(result.Error.StatusCode, result.Error);
            }

            return this.Ok(result.Value);
        }

        [LoggerMessage(Level = LogLevel.Debug, Message = "GET /clients — page {Page}, pageSize {PageSize}")]
        private partial void LogGetPagedClients(int page, int pageSize);

        [LoggerMessage(Level = LogLevel.Information, Message = "POST /clients — creating client {ClientName}")]
        private partial void LogCreateClient(string clientName);

        [LoggerMessage(Level = LogLevel.Warning, Message = "POST /clients — failed to create client {ClientName}: {ErrorCode}")]
        private partial void LogCreateClientFailed(string clientName, string errorCode);

        [LoggerMessage(Level = LogLevel.Information, Message = "POST /clients — client {ClientId} ({ClientName}) created")]
        private partial void LogCreateClientSucceeded(long clientId, string clientName);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "GET /clients/{ClientId}/payments/daily-summary — from {From} to {To}"
        )]
        private partial void LogGetDailySummary(long clientId, DateOnly from, DateOnly to);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "GET /clients/{ClientId}/payments/daily-summary — {ErrorCount} validation error(s)"
        )]
        private partial void LogGetDailySummaryValidationFailed(long clientId, int errorCount);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "GET /clients/{ClientId}/payments/daily-summary — failed: {ErrorCode}"
        )]
        private partial void LogGetDailySummaryFailed(long clientId, string errorCode);
    }
}
