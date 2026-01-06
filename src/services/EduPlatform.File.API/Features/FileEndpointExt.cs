using Asp.Versioning.Builder;
using EduPlatform.File.API.Features.Delete;
using EduPlatform.File.API.Features.Upload;

namespace EduPlatform.File.API.Features;

public static class FileEndpointExt
{
    public static void AddFileGroupEndpointExt(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        app.MapGroup("api/v{version:apiVersion}/files").WithTags("files").WithApiVersionSet(apiVersionSet)
            .UploadFileGroupItemEndpoint()
            .DeleteFileGroupItemEndpoint().RequireAuthorization();
            
    }
}