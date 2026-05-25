using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderHub.Domain.Entities;
using OrderHub.Shared.Constants;

namespace OrderHub.Infrastructure.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="Order"/> entity.
    /// Enforces a unique index on <c>OrderNumber</c>, stores <c>Status</c> as an integer,
    /// and defines a cascade-delete relationship to <see cref="Client"/>.
    /// </summary>
    public sealed class OrderConfiguration : EntityConfiguration<Order, Guid>
    {
        /// <inheritdoc/>
        protected override void ConfigureEntities(EntityTypeBuilder<Order> builder)
        {
            builder
                .Property(entity => entity.OrderNumber)
                .IsRequired()
                .HasMaxLength(OrderConstants.OrderNumberMaxLength);

            builder
                .HasIndex(entity => entity.OrderNumber)
                .IsUnique();
            
            builder
                .Property(entity => entity.Description)
                .IsRequired()
                .HasMaxLength(OrderConstants.DescriptionMaxLength);
            
            builder
                .Property(entity => entity.Status)
                .IsRequired()
                .HasConversion<int>();
            
            builder.HasIndex(entity => entity.ClientId);
            builder.HasIndex(entity => entity.Status);
            
            builder.HasOne<Client>(entity => entity.Client)
                .WithMany()
                .HasForeignKey(entity => entity.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}