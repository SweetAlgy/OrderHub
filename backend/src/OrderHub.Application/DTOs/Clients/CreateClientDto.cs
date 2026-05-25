using System.ComponentModel.DataAnnotations;
using OrderHub.Shared.Constants;

namespace OrderHub.Application.DTOs.Clients
{
    /// <summary>
    /// Payload for creating a new client.
    /// </summary>
    public record CreateClientDto
    {
        /// <summary>
        /// Gets the display name of the new client.
        /// Must not be empty and must not exceed <see cref="ClientConstants.NameMaxLength"/> characters.
        /// </summary>
        [Required]
        [MaxLength(ClientConstants.NameMaxLength)]
        public string Name { get; init; } = String.Empty;
    }
}