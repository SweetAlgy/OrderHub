using OrderHub.Shared.Entities;

namespace OrderHub.Domain.Entities
{
    /// <summary>
    /// Represents a client (customer) in the system.
    /// </summary>
    public sealed class Client : IEntity<long>
    {
        /// <inheritdoc/>
        public long Id { get; init; }

        /// <summary>Gets the display name of the client.</summary>
        public string Name { get; init; } = String.Empty;
    }
}