using System.Net;
using EduPlatform.Shared;
using MediatR;
using Microsoft.Extensions.FileProviders;

namespace EduPlatform.File.API.Features.Upload;

public class UploadFileCommandHandler(IFileProvider fileProvider)
    : IRequestHandler<UploadFileCommand, ServiceResult<UploadFileCommandResponse>>
{
    public async Task<ServiceResult<UploadFileCommandResponse>> Handle(UploadFileCommand request,
        CancellationToken cancellationToken)
    {
        var allowedTypes = new[]
        {
            "application/pdf",
            "image/jpeg",
            "image/png"
        };
        if (request.File.Length == 0)
            return ServiceResult<UploadFileCommandResponse>.Error("Invalid file", "The provided file is empty or null",
                HttpStatusCode.BadRequest);
        
        if (!allowedTypes.Contains(request.File.ContentType))
            return ServiceResult<UploadFileCommandResponse>.Error("Invalid file", "Only PDF, JPG ve PNG tpye can be uploaded.",
            HttpStatusCode.BadRequest);

        var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}"; // .jpg

        var uploadPath = Path.Combine(fileProvider.GetFileInfo("files").PhysicalPath!, newFileName);
        
        await using var stream = new FileStream(uploadPath, FileMode.Create);


        await request.File.CopyToAsync(stream, cancellationToken);


        var response = new UploadFileCommandResponse(newFileName, $"files/{newFileName}", request.File.FileName);

        return ServiceResult<UploadFileCommandResponse>.SuccessAsCreated(response, response.FilePath);
    }
}