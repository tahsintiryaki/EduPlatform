using EduPlatform.Bus.Command;
using EduPlatform.Bus.Event;
using MassTransit;
using Microsoft.Extensions.FileProviders;


namespace EduPlatform.File.API.Consumers;

public class UploadCoursePictureCommandConsumer(IServiceProvider serviceProvider)
    : IConsumer<UploadCoursePictureCommand>
{
    public async Task Consume(ConsumeContext<UploadCoursePictureCommand> context)
    {
        Console.WriteLine("consume UploadCoursePictureCommand from file service");

        using var scope = serviceProvider.CreateScope();
        var fileProvider = scope.ServiceProvider.GetRequiredService<IFileProvider>();

        var newFileName =
            $"{Guid.NewGuid()}{Path.GetExtension(context.Message.FileName)}";

        var root = ((PhysicalFileProvider)fileProvider).Root;
        var uploadPath = Path.Combine(root, newFileName);
        await System.IO.File.WriteAllBytesAsync(uploadPath, context.Message.Picture);


        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        Console.WriteLine("publisher CoursePictureUploadedEvent from file service");

        await publishEndpoint.Publish(
            new CoursePictureUploadedEvent(
                context.Message.CourseId,
                $"files/{newFileName}"
            ));
        ;
    }
}