using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderHub.Application.Interfaces.Services;
using OrderHub.Application.Services;
using OrderHub.Shared.Interfaces.Services;

namespace OrderHub.Application
{
    public sealed class ServicesInstaller : IServicesInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IClientPaymentService, ClientPaymentService>();
            services.AddScoped<IClientService, ClientService>();
        }
    }
}