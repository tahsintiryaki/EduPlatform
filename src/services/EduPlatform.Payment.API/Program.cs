using EduPlatform.Payment.API;
using EduPlatform.Payment.API.Repositories;
using EduPlatform.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using UdemyNewMicroservice.Payment.Api.Feature.Payments;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCommonServiceExt(typeof(PaymentAssembly));
builder.Services.AddVersioningExt();
builder.Services.AddCommonServiceExt(typeof(PaymentAssembly));
builder.Services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("payment-in-memory-db"); });

var app = builder.Build();
app.AddPaymentGroupEndpointExt(app.AddVersionSetExt());
 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();

 