using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderHub.Api.Extensions;
using OrderHub.Api.Transformers;
using OrderHub.Infrastructure;
using OrderHub.Infrastructure.Hubs;
using SwaggerThemes;
using ServicesInstaller = OrderHub.Application.ServicesInstaller;

namespace OrderHub.Api
{
    internal static class Program
    {
        internal static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
                }
            );

            Program.ConfigureServices(builder.Services, builder.Configuration);

            var application = builder.Build();
            Program.Configure(application);

            await application.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSignalR();
            services.AddCors(options =>
                {
                    options.AddPolicy(
                        "AllowFrontend",
                        policy =>
                            policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                    );
                }
            );
            services
                .AddControllers(options =>
                    {
                        options.Conventions.Add(new RouteTokenTransformerConvention(new LowerCaseRouteTransformer()));
                    }
                )
                .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.WriteIndented = true;
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    }
                );

            services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new() { Title = "OrderHub.Api", Version = "v1" });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);
                    options.EnableAnnotations();
                    options.CustomSchemaIds(SwaggerGenericSchemaFormatter.Format);
                    options.DescribeAllParametersInCamelCase();
                    options.UseInlineDefinitionsForEnums();
                }
            );

            Program.InstallServicesFromAssembly(services, configuration);
        }

        private static void Configure(WebApplication application)
        {
            var environment = application.Environment;
            
            Program.ApplyDatabaseMigrations(application);

            if (environment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
                application.UseSwagger();
                application.UseSwaggerUI(Theme.Vs2022);
            }

            if (environment.IsProduction())
            {
                application.UseExceptionHandler("/error");
                application.UseHsts();
                application.UseHttpsRedirection();
            }

            application.UseCors("AllowFrontend");
            application.UseRouting();
            application.MapControllers();

            application.MapHub<OrderStatusHub>("/hubs/orders");
        }

        private static void InstallServicesFromAssembly(IServiceCollection services, IConfiguration configuration)
        {
            new ServicesInstaller().InstallServices(services, configuration);
            new Infrastructure.ServicesInstaller().InstallServices(services, configuration);
        }

        private static void ApplyDatabaseMigrations(WebApplication application)
        {
            var logger = application.Logger;

            try
            {
                logger.LogInformation("Starting database migrations...");
                using var databaseMigrationScope = application.Services.CreateScope();
                var context = databaseMigrationScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
                logger.LogInformation("Database migrations succeeded.");
            }
            catch (NpgsqlException exception)
            {
                logger.LogError(
                    exception,
                    "Connection to the database failed."
                    + "Please ensure that the database is running and accessible."
                );
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An unknown error occurred.");
            }
        }
    }
}