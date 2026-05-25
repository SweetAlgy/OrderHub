using OrderHub.Application.DTOs.Payments;
using OrderHub.Domain.Entities;
using OrderHub.Shared.Results;

namespace OrderHub.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines persistence operations for <see cref="ClientPayment"/> records.
    /// </summary>
    public interface IClientPaymentRepository
    {
        /// <summary>
        /// Retrieves daily payment totals for the specified client within the given date range,
        /// grouped by day.
        /// </summary>
        /// <param name="clientId">The identifier of the client whose payments are queried.</param>
        /// <param name="from">The start date of the range (inclusive).</param>
        /// <param name="to">The end date of the range (inclusive).</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A success result containing a sequence of <see cref="DailyPaymentDto"/> records,
        /// or a 404 failure result if the client does not exist.
        /// </returns>
        public Task<Result<IEnumerable<DailyPaymentDto>>> GetDailyPaymentsAsync(
            long clientId,
            DateOnly from,
            DateOnly to,
            CancellationToken cancellationToken = default
        );
    }
}