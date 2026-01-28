using EduPlatform.Payment.API;
using EduPlatform.Payment.API.Feature.Payments;
using EduPlatform.Payment.API.Repositories;
using EduPlatform.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCommonServiceExt(typeof(PaymentAssembly));

builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);

builder.Services.AddVersioningExt();
builder.Services.AddCommonServiceExt(typeof(PaymentAssembly));
 // builder.Services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("payment-in-memory-db"); });
builder.Services.AddDbContext<InboxDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("payment-db"));
});

builder.Services.AddPaymentMasstransitExt(builder.Configuration);
var app = builder.Build();

app.MapDefaultEndpoints();
app.UseExceptionHandler(x => { });
app.AddPaymentGroupEndpointExt(app.AddVersionSetExt());
 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var dbContext = serviceProvider.GetRequiredService<InboxDbContext>();
    await dbContext.Database.MigrateAsync();
}
app.UseAuthentication();
app.UseAuthorization();

app.Run();

 