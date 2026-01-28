using System.Globalization;
using EduPlatform.Web.DelegateHandlers;
using EduPlatform.Web.ExceptionHandlers;
using EduPlatform.Web.Extensions;
using EduPlatform.Web.Options;
using EduPlatform.Web.Pages.Auth.SignIn;
using EduPlatform.Web.Pages.Auth.SignUp;
using EduPlatform.Web.Services;
using EduPlatform.Web.Services.Refit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMvc(opt => opt.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddOptionsExt();

builder.Services.AddHttpClient<SignUpService>();
builder.Services.AddHttpClient<SignInService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BasketService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<AuthenticatedHttpClientHandler>();
builder.Services.AddScoped<ClientAuthenticatedHttpClientHandler>();
builder.Services.AddExceptionHandler<UnauthorizedAccessExceptionHandler>();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddRefitClient<ICatalogRefitService>().ConfigureHttpClient(configure =>
    {
        var microserviceOption = builder.Configuration.GetSection(nameof(MicroserviceOption)).Get<MicroserviceOption>();
        configure.BaseAddress = new Uri("http://eduplatform-catalog-api");
    }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
    .AddHttpMessageHandler<ClientAuthenticatedHttpClientHandler>();


builder.Services.AddRefitClient<IBasketRefitService>().ConfigureHttpClient(configure =>
    {
        var microserviceOption = builder.Configuration.GetSection(nameof(MicroserviceOption)).Get<MicroserviceOption>();
        configure.BaseAddress = new Uri("http://eduplatform-basket-api");
    }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
    .AddHttpMessageHandler<ClientAuthenticatedHttpClientHandler>();


builder.Services.AddRefitClient<IDiscountRefitService>().ConfigureHttpClient(configure =>
    {
        var microserviceOption = builder.Configuration.GetSection(nameof(MicroserviceOption)).Get<MicroserviceOption>();
        configure.BaseAddress = new Uri("http://eduplatform-discount-api");
    }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
    .AddHttpMessageHandler<ClientAuthenticatedHttpClientHandler>();


builder.Services.AddRefitClient<IOrderRefitService>().ConfigureHttpClient(configure =>
    {
        var microserviceOption = builder.Configuration.GetSection(nameof(MicroserviceOption)).Get<MicroserviceOption>();
        configure.BaseAddress = new Uri("http://eduplatform-order-api");
    }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
    .AddHttpMessageHandler<ClientAuthenticatedHttpClientHandler>();

builder.Services.AddRefitClient<IPaymentRefitService>().ConfigureHttpClient(configure =>
    {
        var microserviceOption = builder.Configuration.GetSection(nameof(MicroserviceOption)).Get<MicroserviceOption>();
        configure.BaseAddress = new Uri("http://eduplatform-payment-api");
    }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
    .AddHttpMessageHandler<ClientAuthenticatedHttpClientHandler>();



builder.Services.AddAuthentication(configureOption =>
    {
        configureOption.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        configureOption.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/auth/signin";
        options.ExpireTimeSpan = TimeSpan.FromDays(60);
        options.Cookie.Name = "EduPlatformWebCookie";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });


builder.Services.AddAuthorization();
var app = builder.Build();

app.MapDefaultEndpoints();

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = [cultureInfo],
    SupportedUICultures = [cultureInfo]
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();