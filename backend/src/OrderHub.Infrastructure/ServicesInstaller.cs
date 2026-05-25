using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderHub.Application.Interfaces.Repositories;
using OrderHub.Application.Interfaces.Services;
using OrderHub.Infrastructure.Repositories;
using OrderHub.Infrastructure.Services;
using OrderHub.Shared.Interfaces.Services;

namespace OrderHub.Infrastructure
{
    public sealed class ServicesInstaller : IServicesInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IClientPaymentRepository, ClientPaymentRepository>();
            services.AddScoped<IOrderNotificationService, OrderNotificationService>();
        }
    }
}