using System.ComponentModel.DataAnnotations;

namespace OrderHub.Application.DTOs.Orders
{
    /// <summary>
    /// Query parameters for retrieving a client's daily payment summary within a date range.
    /// </summary>
    public record GetDailyPaymentsRequest
    {
        /// <summary>Gets the start date of the range (inclusive).</summary>
        [Required]
        public DateOnly From { get; init; }

        /// <summary>Gets the end date of the range (inclusive).</summary>
        [Required]
        public DateOnly To { get; init; }

        /// <summary>
        /// Performs business-rule validation beyond data-annotation checks.
        /// Currently verifies that <see cref="From"/> is not later than <see cref="To"/>.
        /// </summary>
        /// <returns>
        /// A sequence of <see cref="ValidationResult"/> instances describing any violations,
        /// or an empty sequence when the request is valid.
        /// </returns>
        public IEnumerable<ValidationResult> Validate()
        {
            if (this.From > this.To)
                yield return new(
                    "'From' date must be earlier than or equal to 'To' date.",
                    [nameof(GetDailyPaymentsRequest.From), nameof(GetDailyPaymentsRequest.To)]);
        }
    }
}