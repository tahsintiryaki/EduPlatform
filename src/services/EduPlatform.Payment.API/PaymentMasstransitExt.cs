using EduPlatform.Bus;

namespace EduPlatform.Payment.API;

public static class PaymentMasstransitExt
{
    public static IServiceCollection AddPaymentMasstransitExt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddRabbitMqBusExt(
            configuration,
            consumerAssemblies: new[] { typeof(PaymentAssembly).Assembly },
            endpointPrefix: "payment."
        );
    }
}