using EduPlatform.Catalog.API.Repositories;

namespace EduPlatform.Catalog.API.Features.Courses.Update;

public class UpdateCourseCommandHandler(AppDbContext context, IMapper mapper)
    : IRequestHandler<UpdateCourseCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var courseToUpdate = await context.Courses.FindAsync([request.Id], cancellationToken);
        if (courseToUpdate == null) return ServiceResult.ErrorAsNotFound();

        courseToUpdate.Name = request.Name;
        courseToUpdate.Description = request.Description;
        courseToUpdate.Price = request.Price;
        courseToUpdate.ImageUrl = request.ImageUrl;
        courseToUpdate.CategoryId = request.CategoryId;


        context.Courses.Update(courseToUpdate);


        await context.SaveChangesAsync(cancellationToken);

        return ServiceResult.SuccessAsNoContent();
    }
}