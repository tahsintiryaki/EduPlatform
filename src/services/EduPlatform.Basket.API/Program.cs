using EduPlatform.Basket.API;
using EduPlatform.Basket.API.Feature.Baskets;
using EduPlatform.Bus;
using EduPlatform.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCommonServiceExt(typeof(BasketAssembly)); 
builder.Services.AddBasketMasstransitExt(builder.Configuration);
builder.Services.AddVersioningExt();
builder.Services.AddScoped<BasketService>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
var app = builder.Build();
app.AddBasketGroupEndpointExt(app.AddVersionSetExt());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.Run();

