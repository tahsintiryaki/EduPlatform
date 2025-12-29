using EduPlatform.Catalog.API.Repositories;

namespace EduPlatform.Catalog.API.Features.Courses.Create;

public class CreateCourseCommandHandler(AppDbContext context, IMapper mapper)
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
        newCourse.Id = NewId.NextSequentialGuid(); // index performance

        newCourse.Feature = new Feature
        {
            Duration = 10, // calculate by course video
            EducatorFullName = "Ahmet Yılmaz", // get by token payload
            Rating = 0
        };
        await context.Courses.AddAsync(newCourse);
        await context.SaveChangesAsync(cancellationToken);
        return ServiceResult<Guid>.SuccessAsCreated(newCourse.Id, $"/api/courses/{newCourse.Id}");
    }
}