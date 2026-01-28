using EduPlatform.Bus;
using EduPlatform.Order.API.Endpoints.Orders;
using EduPlatform.Order.application;
using EduPlatform.Order.Application;
using EduPlatform.Order.Application.Contracts.Refit;
using EduPlatform.Order.Application.Contracts.Repositories;
using EduPlatform.Order.Application.UnitOfWork;
using EduPlatform.Order.Persistence;
using EduPlatform.Order.Persistence.Repositories;
using EduPlatform.Order.Persistence.UnitOfWork;
using EduPlatform.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCommonServiceExt(typeof(OrderApplicationAssembly));
 

// builder.Services.AddDbContext<AppDbContext>(option =>
// {
//     option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
// });

// db name is same from aspire order api configuration
builder.AddSqlServerDbContext<AppDbContext>("order-db");
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderOutboxRepository, OrderOutboxRepository>();
builder.Services.AddScoped<IPaymentInboxRepository, PaymentInboxRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddVersioningExt();
builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
builder.Services.AddRefitConfigurationExt(builder.Configuration);
builder.Services.AddOrderMasstransitExt(builder.Configuration);
// builder.Services.AddHostedService<CheckPaymentStatusOrderBackgroundService>();

var app = builder.Build();

app.MapDefaultEndpoints();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}
app.UseExceptionHandler(x => { });
app.AddOrderGroupEndpointExt(app.AddVersionSetExt());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();


app.Run();

