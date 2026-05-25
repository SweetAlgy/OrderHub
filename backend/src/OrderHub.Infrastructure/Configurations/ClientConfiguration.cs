using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderHub.Domain.Entities;
using OrderHub.Shared.Constants;

namespace OrderHub.Infrastructure.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="Client"/> entity.
    /// Applies column constraints on top of the common base configuration.
    /// </summary>
    public sealed class ClientConfiguration : EntityConfiguration<Client, long>
    {
        /// <inheritdoc/>
        protected override void ConfigureEntities(EntityTypeBuilder<Client> builder)
        {
            builder
                .Property(entity => entity.Name)
                .IsRequired()
                .HasMaxLength(ClientConstants.NameMaxLength);
        }
    }
}