#region

using EduPlatform.Web.Options;
using Microsoft.Extensions.Options;

#endregion

namespace EduPlatform.Web.Extensions;

public static class OptionsExt
{
    public static IServiceCollection AddOptionsExt(this IServiceCollection services)
    {
        services.AddOptions<IdentityOption>().BindConfiguration(nameof(IdentityOption)).ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IdentityOption>(sp => sp.GetRequiredService<IOptions<IdentityOption>>().Value);
        
        services.AddOptions<MicroserviceOption>().BindConfiguration(nameof(MicroserviceOption))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<MicroserviceOption>(sp => sp.GetRequiredService<IOptions<MicroserviceOption>>().Value);
        return services;
    }
}