using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderHub.Domain.Entities;

namespace OrderHub.Infrastructure.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="ClientPayment"/> entity.
    /// Stores <c>Amount</c> as <c>NUMERIC(18, 2)</c>, creates a composite index on
    /// <c>(ClientId, PaymentDate)</c>, and defines a restrict-delete relationship to <see cref="Client"/>.
    /// </summary>
    public sealed class ClientPaymentConfiguration : EntityConfiguration<ClientPayment, long>
    {
        /// <inheritdoc/>
        protected override void ConfigureEntities(EntityTypeBuilder<ClientPayment> builder)
        {
            builder
                .Property(entity => entity.PaymentDate)
                .IsRequired();

            builder
                .Property(entity => entity.Amount)
                .IsRequired()
                .HasColumnType("NUMERIC(18, 2)");

            builder.HasIndex(entity => new { entity.ClientId, entity.PaymentDate });
            
            builder.HasOne<Client>()
                .WithMany()
                .HasForeignKey(entity => entity.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}