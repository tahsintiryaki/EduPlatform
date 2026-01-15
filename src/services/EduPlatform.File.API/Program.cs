using EduPlatform.Bus;
using EduPlatform.File.API;
using EduPlatform.File.API.Features;
using EduPlatform.Shared.Extensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IFileProvider>(
    new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

builder.Services.AddCommonServiceExt(typeof(FileAssembly));
builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
builder.Services.AddFileMasstransitExt(builder.Configuration);
builder.Services.AddVersioningExt();
var app = builder.Build();
app.UseExceptionHandler(x => { });

app.AddFileGroupEndpointExt(app.AddVersionSetExt());

app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.Run();