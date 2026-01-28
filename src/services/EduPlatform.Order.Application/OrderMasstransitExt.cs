using EduPlatform.Bus;
using EduPlatform.Order.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduPlatform.Order.application;

public static class OrderMasstransitExt
{
    public static IServiceCollection AddOrderMasstransitExt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddRabbitMqBusExt(
            configuration,
            consumerAssemblies: new[] { typeof(OrderApiAssembly).Assembly },
            endpointPrefix: "order."
        );
    }
}