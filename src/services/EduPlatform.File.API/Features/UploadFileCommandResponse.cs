namespace EduPlatform.File.API.Features;

public record UploadFileCommandResponse(string FileName, string FilePath, string OriginalFileName);