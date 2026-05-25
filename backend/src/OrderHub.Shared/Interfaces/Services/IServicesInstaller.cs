using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderHub.Shared.Interfaces.Services
{
    /// <summary>
    /// Defines a contract for layer-level service registration modules.
    /// Each application layer provides its own implementation that registers
    /// the services it owns into the DI container.
    /// </summary>
    public interface IServicesInstaller
    {
        /// <summary>
        /// Registers the services owned by this installer into the <paramref name="services"/> collection.
        /// </summary>
        /// <param name="services">The DI service collection to populate.</param>
        /// <param name="configuration">The application configuration used to read settings.</param>
        public void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}