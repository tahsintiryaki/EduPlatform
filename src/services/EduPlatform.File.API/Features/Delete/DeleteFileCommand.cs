using EduPlatform.Shared;

namespace EduPlatform.File.API.Features.Delete;

public record DeleteFileCommand(string FileName) : IRequestByServiceResult;