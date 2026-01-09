using EduPlatform.Bus;

namespace EduPlatform.Basket.API;

public static class BasketMasstransitExt
{
    public static IServiceCollection AddBasketMasstransitExt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddRabbitMqBusExt(
            configuration,
            consumerAssemblies: new[] { typeof(BasketAssembly).Assembly },
            endpointPrefix: "basket."
        );
    }
}