using EduPlatform.Catalog.API;
using EduPlatform.Catalog.API.Features.Categories;
using EduPlatform.Catalog.API.Options;
using EduPlatform.Catalog.API.Repositories;
using EduPlatform.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptionExt();//mongodb connection value  validate
builder.Services.AddMongoDbConfigureExt();//connect mongodb  
builder.Services.AddCommonServiceExt(typeof(CatalogAssembly)); //


var app = builder.Build();


app.AddCategoryGroupEndpointExt();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();