using EduPlatform.Bus;
using EduPlatform.File.API;
using EduPlatform.File.API.Features;
using EduPlatform.Shared.Extensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var storageRoot =
    builder.Configuration["Storage:Root"]
    ?? Path.Combine(builder.Environment.ContentRootPath, "storage");

var filesRoot = Path.Combine(storageRoot, "files");

Directory.CreateDirectory(filesRoot);

builder.Services.AddSingleton<IFileProvider>(
    new PhysicalFileProvider(filesRoot));

builder.Services.AddCommonServiceExt(typeof(FileAssembly));
builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
builder.Services.AddFileMasstransitExt(builder.Configuration);
builder.Services.AddVersioningExt();
var app = builder.Build();
app.UseExceptionHandler(x => { });

app.AddFileGroupEndpointExt(app.AddVersionSetExt());

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(filesRoot),
    RequestPath = "/files"
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.Run();