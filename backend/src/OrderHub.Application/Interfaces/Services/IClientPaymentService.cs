using OrderHub.Application.DTOs.Payments;
using OrderHub.Shared.Results;

namespace OrderHub.Application.Interfaces.Services
{
    /// <summary>
    /// Defines business operations for querying client payment data.
    /// </summary>
    public interface IClientPaymentService
    {
        /// <summary>
        /// Retrieves daily payment totals for the specified client within the given date range.
        /// </summary>
        /// <param name="clientId">The identifier of the client whose payments are queried.</param>
        /// <param name="from">The start date of the range (inclusive).</param>
        /// <param name="to">The end date of the range (inclusive).</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A success result containing a sequence of <see cref="DailyPaymentDto"/> records grouped by day,
        /// or a 404 failure result if the client does not exist.
        /// </returns>
        public Task<Result<IEnumerable<DailyPaymentDto>>> GetDailyPaymentsAsync(
            long clientId,
            DateOnly from,
            DateOnly to,
            CancellationToken cancellationToken = default);
    }
}