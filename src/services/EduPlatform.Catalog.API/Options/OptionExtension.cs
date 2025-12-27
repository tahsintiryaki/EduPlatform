namespace EduPlatform.Catalog.API.Options;

public static class OptionExtension
{
    public static IServiceCollection AddOptionExt(this IServiceCollection services)
    {
        services.AddOptions<MongoOption>().BindConfiguration(nameof(MongoOption))
            .ValidateDataAnnotations() 
            .ValidateOnStart(); 
        return services;
    }
    
}