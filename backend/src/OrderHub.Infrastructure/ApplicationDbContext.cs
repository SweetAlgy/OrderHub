using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderHub.Domain.Entities;
using OrderHub.Shared.Entities;

namespace OrderHub.Infrastructure
{
    /// <summary>
    /// EF Core database context for the OrderHub application.
    /// Uses PostgreSQL with snake_case column naming conventions.
    /// Entity-type configurations are loaded automatically from this assembly.
    /// </summary>
    public class ApplicationDbContext(IConfiguration configuration) : DbContext
    {
        private readonly string _connectionString = configuration.GetConnectionString("PostgresConnection")
                                                    ?? throw new InvalidOperationException(
                                                        "Postgres connection string is not set"
                                                    );

        /// <summary>Gets the <see cref="Client"/> entity set.</summary>
        public DbSet<Client> Clients => this.Set<Client>();

        /// <summary>Gets the <see cref="Order"/> entity set.</summary>
        public DbSet<Order> Orders => this.Set<Order>();

        /// <summary>Gets the <see cref="ClientPayment"/> entity set.</summary>
        public DbSet<ClientPayment> ClientPayments => this.Set<ClientPayment>();

        /// <summary>
        /// Configures the database provider and naming conventions when the context
        /// has not already been configured externally (e.g. via DI options).
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            optionsBuilder.UseSnakeCaseNamingConvention();
            optionsBuilder.UseNpgsql(this._connectionString);

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Applies all <see cref="IEntityTypeConfiguration{TEntity}"/> implementations found in
        /// the Infrastructure assembly and registers audit-timestamp properties for every entity
        /// that implements <see cref="ITrackableEntity{T}"/>.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ApplicationDbContext.ConfigureEntities(modelBuilder);
        }

        /// <summary>
        /// Scans the assembly for <see cref="IEntityTypeConfiguration{TEntity}"/> implementations
        /// and additionally ensures that <c>CreatedAt</c> and <c>UpdatedAt</c> columns are mapped
        /// for every <see cref="ITrackableEntity{T}"/> entity type.
        /// </summary>
        private static void ConfigureEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                         .Where(entityType => entityType.ClrType
                             .GetInterfaces()
                             .Any(i => i.IsGenericType &&
                                       i.GetGenericTypeDefinition() == typeof(ITrackableEntity<>))))
            {
                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property(nameof(ITrackableEntity<>.CreatedAt));
                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property(nameof(ITrackableEntity<>.UpdatedAt));
            }
        }
    }
}