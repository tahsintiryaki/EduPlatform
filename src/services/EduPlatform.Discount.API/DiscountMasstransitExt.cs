using EduPlatform.Bus;
using EduPlatform.Discount.API;

namespace EduPlatform.Discount.API;

public static class DiscountMasstransitExt
{
    public static IServiceCollection AddDiscountMasstransitExt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddRabbitMqBusExt(
            configuration,
            consumerAssemblies: new[] { typeof(DiscountAssembly).Assembly },
            endpointPrefix: "discount."
        );
    }
}