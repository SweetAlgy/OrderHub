using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderHub.Shared.Entities;
using OrderHub.Shared.Extensions;

namespace OrderHub.Infrastructure.Configurations
{
    /// <summary>
    /// Base EF Core entity configuration that automatically maps the table name and primary key
    /// for any entity implementing <see cref="IEntity{T}"/>.
    /// <para>
    /// The table name is derived from the entity class name: the optional <c>Entity</c> suffix
    /// is stripped, the result is pluralised with Humanizer, and then converted to snake_case.
    /// For example, <c>ClientPayment</c> → <c>client_payments</c>.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type to configure.</typeparam>
    /// <typeparam name="T">The type of the entity's primary key.</typeparam>
    public abstract class EntityConfiguration<TEntity, T> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity<T>
    {
        private const string EntityPostfix = "Entity";

        /// <summary>
        /// Applies common table-name and primary-key configuration, then delegates to
        /// <see cref="ConfigureEntities"/> for entity-specific mappings.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            var entityTypeName = typeof(TEntity).Name;

            if (entityTypeName.EndsWith(EntityConfiguration<TEntity, T>.EntityPostfix))
                entityTypeName = entityTypeName[..^EntityConfiguration<TEntity, T>.EntityPostfix.Length];

            var tableName = entityTypeName
                .Pluralize()
                .ToSnakeCase();

            builder
                .ToTable(tableName)
                .HasKey(entity => entity.Id);

            builder
                .Property(entity => entity.Id)
                .ValueGeneratedOnAdd();

            this.ConfigureEntities(builder);
        }

        /// <summary>
        /// Applies entity-specific column constraints, indexes, and relationships.
        /// Implemented by each concrete configuration class.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        protected abstract void ConfigureEntities(EntityTypeBuilder<TEntity> builder);
    }
}