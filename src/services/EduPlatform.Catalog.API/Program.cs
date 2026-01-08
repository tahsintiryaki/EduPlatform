using EduPlatform.Bus;
using EduPlatform.Catalog.API;
using EduPlatform.Catalog.API.Features.Categories;
using EduPlatform.Catalog.API.Features.Courses;
using EduPlatform.Catalog.API.Options;
using EduPlatform.Catalog.API.Repositories;
using EduPlatform.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptionExt();//mongodb connection value  validate
builder.Services.AddMongoDbConfigureExt();//connect mongodb  
builder.Services.AddCommonServiceExt(typeof(CatalogAssembly)); 
builder.Services.AddCatalogMasstransitExt(builder.Configuration);
builder.Services.AddVersioningExt();
builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
var app = builder.Build();

app.AddSeedDataExt().ContinueWith(x =>
{
    Console.WriteLine(x.IsFaulted ? x.Exception?.Message : "Seed data has been saved successfully");
});
app.AddCategoryGroupEndpointExt(app.AddVersionSetExt());
app.AddCCourseGroupEndpointExt(app.AddVersionSetExt());


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.Run();