using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderHub.Application.DTOs.Payments;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Domain.Entities;
using OrderHub.Shared.Results;

namespace OrderHub.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IClientPaymentRepository"/>.
    /// Daily payment totals are retrieved via the <c>get_daily_payments</c> PostgreSQL function
    /// using a raw SQL query.
    /// </summary>
    public sealed partial class ClientPaymentRepository(
        ApplicationDbContext context,
        ILogger<ClientPaymentRepository> logger
    ) : IClientPaymentRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ClientPaymentRepository> _logger = logger;

        /// <inheritdoc/>
        public async Task<Result<IEnumerable<DailyPaymentDto>>> GetDailyPaymentsAsync(
            long clientId,
            DateOnly from,
            DateOnly to,
            CancellationToken cancellationToken = default
        )
        {
            this.LogQueryingDailyPayments(clientId, from, to);

            var clientExists = await context.Clients
                .AsNoTracking()
                .AnyAsync(client => client.Id == clientId, cancellationToken);

            if (!clientExists)
            {
                this.LogClientNotFoundForPayments(clientId);
                return Result.Failure<IEnumerable<DailyPaymentDto>>(
                    Error.NotFound(nameof(Client), clientId)
                );
            }

            var result = await context.Database
                .SqlQuery<DailyPaymentDto>(
                    $"""
                         SELECT payment_date, amount
                         FROM get_daily_payments({clientId}, {from}, {to})
                     """
                )
                .ToListAsync(cancellationToken);

            this.LogDailyPaymentsQueried(clientId, result.Count);
            return Result.Success<IEnumerable<DailyPaymentDto>>(result);
        }

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Querying daily payments for client {ClientId} from {From} to {To}"
        )]
        private partial void LogQueryingDailyPayments(long clientId, DateOnly from, DateOnly to);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Client {ClientId} not found when querying payments")]
        private partial void LogClientNotFoundForPayments(long clientId);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Daily payments query for client {ClientId} returned {Count} records")]
        private partial void LogDailyPaymentsQueried(long clientId, int count);
    }
}
