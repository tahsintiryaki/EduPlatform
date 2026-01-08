#region

using EduPlatform.Bus.Event;
using EduPlatform.Catalog.API.Repositories;

#endregion

namespace EduPlatform.Catalog.API.Consumers;

public class UpdateCourseImageUrlOnCoursePictureUploadedConsumer(IServiceProvider serviceProvider)
    : IConsumer<CoursePictureUploadedEvent>
{
    public async Task Consume(ConsumeContext<CoursePictureUploadedEvent> context)
    {
        Console.WriteLine("consume CoursePictureUploadedEvent from catalog service");
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var course = dbContext.Courses.Find(context.Message.CourseId);
        if (course == null) throw new NotImplementedException();
        course.ImageUrl = context.Message.ImageUrl;
        await dbContext.SaveChangesAsync();
    }
}