using EduPlatform.Catalog.API.Options;
using EduPlatform.Catalog.API.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//mongodb connection value  validate
builder.Services.AddOptionExt();
//connect mongodb  
builder.Services.AddMongoDbConfigureExt();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();