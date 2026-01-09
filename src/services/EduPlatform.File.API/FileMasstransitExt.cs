using EduPlatform.Bus;

namespace EduPlatform.File.API;

public static class FileMasstransitExt
{
    public static IServiceCollection AddFileMasstransitExt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddRabbitMqBusExt(
            configuration,
            consumerAssemblies: new[] { typeof(FileAssembly).Assembly },
            endpointPrefix: "file"
        );
    }
}