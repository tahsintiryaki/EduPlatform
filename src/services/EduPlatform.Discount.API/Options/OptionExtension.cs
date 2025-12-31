using Microsoft.Extensions.Options;

namespace EduPlatform.Discount.API.Options;

public static class OptionExtension
{
    public static IServiceCollection AddOptionExt(this IServiceCollection services)
    {
        services.AddOptions<MongoOption>().BindConfiguration(nameof(MongoOption))
            .ValidateDataAnnotations() 
            .ValidateOnStart(); 
        
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoOption>>().Value);
        return services;
    }
    
}