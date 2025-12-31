using EduPlatform.Shared;

namespace EduPlatform.File.API.Features.Upload;

public record UploadFileCommand(IFormFile File) : IRequestByServiceResult<UploadFileCommandResponse>;