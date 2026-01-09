#region

using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace EduPlatform.Bus;

public static class MasstransitConfigurationExt
{
    public static IServiceCollection AddRabbitMqBusExt(this IServiceCollection services,
        IConfiguration configuration, Assembly[]? consumerAssemblies = null,
        string? endpointPrefix = null)
    {
        var busOptions = configuration.GetSection(nameof(BusOption)).Get<BusOption>()!;

        services.AddMassTransit(configure =>
        {
            var existConsumerAssemblies = consumerAssemblies != null;
            // İlgili assembly’lerdeki tüm consumer’ları tara
            if (existConsumerAssemblies)
            {
                foreach (var assembly in consumerAssemblies!)
                {
                    configure.AddConsumers(assembly);
                }
            }

            configure.SetEndpointNameFormatter(
                new KebabCaseEndpointNameFormatter(prefix: endpointPrefix, includeNamespace: false));


            configure.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(new Uri($"rabbitmq://{busOptions.Address}:{busOptions.Port}"), host =>
                {
                    host.Username(busOptions.UserName);
                    host.Password(busOptions.Password);
                    
                    host.Heartbeat(120);
                    host.RequestedConnectionTimeout(TimeSpan.FromSeconds(30));
                });
               
                cfg.ConfigureEndpoints(ctx);
                
                cfg.UseCircuitBreaker(configurator =>
                {
                    configurator.TrackingPeriod = TimeSpan.FromMinutes(1);
                    configurator.TripThreshold = 15;
                    configurator.ActiveThreshold = 10;
                    configurator.ResetInterval = TimeSpan.FromMinutes(5);
                });
                cfg.UseMessageRetry(r => r.Immediate(5));
            });
        });
        return services;
    }
}