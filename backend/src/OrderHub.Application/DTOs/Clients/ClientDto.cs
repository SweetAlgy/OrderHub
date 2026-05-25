namespace OrderHub.Application.DTOs.Clients
{
    /// <summary>
    /// Read model returned by the API to represent a client.
    /// </summary>
    public record ClientDto
    {
        /// <summary>Gets the unique identifier of the client.</summary>
        public long Id { get; init; }

        /// <summary>Gets the display name of the client.</summary>
        public string Name { get; init; } = String.Empty;
    }
}