using EduPlatform.Bus.Command;
using EduPlatform.Catalog.API.Repositories;
using EduPlatform.Shared.Services;

namespace EduPlatform.Catalog.API.Features.Courses.Create;

public class CreateCourseCommandHandler(
    AppDbContext context,
    IMapper mapper,
    IIdentityService identityService,
    ISendEndpointProvider sendEndpointProvider)
    : IRequestHandler<CreateCourseCommand, ServiceResult<Guid>>
{
    public async Task<ServiceResult<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await context.Categories.AnyAsync(t => t.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
            return ServiceResult<Guid>.Error("Category not found",
                $"The category with Id:{request.CategoryId} not found ", HttpStatusCode.NotFound);

        var courseExists = await context.Courses.AnyAsync(x => x.Name == request.Name, cancellationToken);

        if (courseExists)
            return ServiceResult<Guid>.Error("Course already exists.",
                $"The Course with name({request.Name}) already exists", HttpStatusCode.BadRequest);

        var newCourse = mapper.Map<Course>(request);
        newCourse.Created = DateTime.Now;
        newCourse.UserId = identityService.UserId;
        newCourse.Id = NewId.NextSequentialGuid(); // index performance

        newCourse.Feature = new Feature
        {
            Duration = 10, // calculate by course video
            EducatorFullName = "Ahmet Yılmaz", // get by token payload
            Rating = 0
        };
        await context.Courses.AddAsync(newCourse);
        await context.SaveChangesAsync(cancellationToken);
        if (request.Picture is not null)
        {
            using var memoryStream = new MemoryStream();
            await request.Picture.CopyToAsync(memoryStream, cancellationToken);
            var pictureBytes = memoryStream.ToArray();

            UploadCoursePictureCommand uploadCoursePictureCommand =
                new(newCourse.Id, pictureBytes, request.Picture.FileName);
            
            var endpoint = await sendEndpointProvider.GetSendEndpoint(
                new Uri("queue:file-upload-course-picture-command"));

            await endpoint.Send(uploadCoursePictureCommand, cancellationToken);
            Console.WriteLine("publisher UploadCoursePictureCommand from CatalogService");
        }

        return ServiceResult<Guid>.SuccessAsCreated(newCourse.Id, $"/api/courses/{newCourse.Id}");
    }
}