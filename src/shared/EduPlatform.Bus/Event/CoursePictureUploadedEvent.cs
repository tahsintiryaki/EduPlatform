namespace EduPlatform.Bus.Event;

public record CoursePictureUploadedEvent(Guid CourseId, string ImageUrl);