using EduPlatform.Catalog.API.Features.Courses.Dtos;
using EduPlatform.Catalog.API.Repositories;

namespace EduPlatform.Catalog.API.Features.Courses.GetById;

public record GetCourseByIdQuery(Guid Id) : IRequestByServiceResult<CourseDto>;

public class GetCourseByIdQueryHandler(AppDbContext context, IMapper mapper)
    : IRequestHandler<GetCourseByIdQuery, ServiceResult<CourseDto>>
{
    public async Task<ServiceResult<CourseDto>> Handle(GetCourseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var courseExists = await context.Courses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);


        if (courseExists is null)
            return ServiceResult<CourseDto>.Error("Course not found",
                $"The course with id({request.Id}) was not found", HttpStatusCode.NotFound);

        var category = await context.Categories.FindAsync(courseExists.CategoryId, cancellationToken);

        courseExists.Category = category!;


        var courseAsDto = mapper.Map<CourseDto>(courseExists);
        return ServiceResult<CourseDto>.SuccessAsOk(courseAsDto);
    }
}

public static class GetCourseByIdEndpoint
{
    public static RouteGroupBuilder GetByIdCourseGroupItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}",
                async (IMediator mediator, Guid id) =>
                    (await mediator.Send(new GetCourseByIdQuery(id))).ToGenericResult())
            .WithName("GetByIdCourses").MapToApiVersion(1, 0);
          

        return group;
    }
}