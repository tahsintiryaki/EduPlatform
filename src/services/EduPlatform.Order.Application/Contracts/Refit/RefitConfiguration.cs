using EduPlatform.Order.Application.Contracts.Refit.PaymentService;
using EduPlatform.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace EduPlatform.Order.Application.Contracts.Refit;

public static class RefitConfiguration
{
    public static IServiceCollection AddRefitConfigurationExt(this IServiceCollection services, IConfiguration configuration)
    {
        //added interceptor for token 
        services.AddScoped<AuthenticatedHttpClientHandler>();
        services.AddRefitClient<IPaymentService>().ConfigureHttpClient(configure =>
        {
            var addressOption = configuration.GetSection(nameof(AddressUrlOption)).Get<AddressUrlOption>();
            configure.BaseAddress = new Uri(addressOption!.PaymentUrl);
        }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>(); // Intercetpr added to refit requests
        return services;
    }
}