using Microsoft.Extensions.Logging;
using OrderHub.Application.DTOs.Payments;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Application.Interfaces.Services;
using OrderHub.Shared.Results;

namespace OrderHub.Application.Services
{
    /// <summary>
    /// Implements <see cref="IClientPaymentService"/> by delegating to
    /// <see cref="IClientPaymentRepository"/> and adding structured logging.
    /// </summary>
    public sealed partial class ClientPaymentService(
        IClientPaymentRepository paymentRepository,
        ILogger<ClientPaymentService> logger
    ) : IClientPaymentService
    {
        private readonly IClientPaymentRepository _paymentRepository = paymentRepository;
        private readonly ILogger<ClientPaymentService> _logger = logger;

        /// <inheritdoc/>
        public async Task<Result<IEnumerable<DailyPaymentDto>>> GetDailyPaymentsAsync(
            long clientId,
            DateOnly from,
            DateOnly to,
            CancellationToken cancellationToken = default
        )
        {
            this.LogGettingDailyPayments(clientId, from, to);

            var result = await this._paymentRepository.GetDailyPaymentsAsync(clientId, from, to, cancellationToken);

            if (result.IsFailure)
            {
                this.LogGetDailyPaymentsFailed(clientId, result.Error.Code);
                return result;
            }

            this.LogDailyPaymentsLoaded(clientId, result.Value.Count());
            return result;
        }

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Getting daily payments for client {ClientId} from {From} to {To}"
        )]
        private partial void LogGettingDailyPayments(long clientId, DateOnly from, DateOnly to);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to get daily payments for client {ClientId}: {ErrorCode}")]
        private partial void LogGetDailyPaymentsFailed(long clientId, string errorCode);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Loaded {Count} daily payment records for client {ClientId}")]
        private partial void LogDailyPaymentsLoaded(long clientId, int count);
    }
}
