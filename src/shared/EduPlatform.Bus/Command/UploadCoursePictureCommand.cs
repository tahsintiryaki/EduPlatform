namespace EduPlatform.Bus.Command;

public record UploadCoursePictureCommand(Guid CourseId, byte[] Picture, string FileName);