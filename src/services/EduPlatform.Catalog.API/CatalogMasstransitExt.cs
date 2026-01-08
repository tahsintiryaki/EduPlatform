using EduPlatform.Bus;

namespace EduPlatform.Catalog.API;

public static class CatalogMasstransitExt
{
    public static IServiceCollection AddCatalogMasstransitExt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddRabbitMqBusExt(
            configuration,
            consumerAssemblies: new[] { typeof(CatalogAssembly).Assembly },
            endpointPrefix: "catalog"
        );
    }
}