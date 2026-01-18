using EduPlatform.Order.Application.Contracts.Refit.PaymentService;
using EduPlatform.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace EduPlatform.Order.Application.Contracts.Refit;

public static class RefitConfiguration
{
    public static IServiceCollection AddRefitConfigurationExt(this IServiceCollection services, IConfiguration configuration)
    {
       
        
        //added interceptor for token 
        services.AddScoped<AuthenticatedHttpClientHandler>();
        services.AddScoped<ClientAuthenticatedHttpClientHandler>();
        
        services.AddOptions<IdentityOption>().BindConfiguration(nameof(IdentityOption)).ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<IdentityOption>(sp => sp.GetRequiredService<IOptions<IdentityOption>>().Value);

        services.AddOptions<ClientSecretOption>().BindConfiguration(nameof(ClientSecretOption)).ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<ClientSecretOption>(sp => sp.GetRequiredService<IOptions<ClientSecretOption>>().Value);

        
        services.AddRefitClient<IPaymentService>().ConfigureHttpClient(configure =>
        {
            var addressOption = configuration.GetSection(nameof(AddressUrlOption)).Get<AddressUrlOption>();
            configure.BaseAddress = new Uri(addressOption!.PaymentUrl);
        }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>().AddHttpMessageHandler<ClientAuthenticatedHttpClientHandler>(); // Intercetpr added to refit requests
        
        
        return services;
    }
}