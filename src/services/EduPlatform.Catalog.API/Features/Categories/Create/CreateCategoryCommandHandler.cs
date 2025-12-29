using EduPlatform.Catalog.API.Repositories;

namespace EduPlatform.Catalog.API.Features.Categories.Create;

public class CreateCategoryCommandHandler(AppDbContext context)
    : IRequestHandler<CreateCategoryCommand, ServiceResult<CreateCategoryResponse>>
{
    public async Task<ServiceResult<CreateCategoryResponse>> Handle(CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var existCategory = await context.Categories.AnyAsync(t => t.Name == request.Name, cancellationToken);
        if (existCategory)
            return ServiceResult<CreateCategoryResponse>.Error("Category already exists",
                $"The category name {request.Name} already exist", HttpStatusCode.BadRequest);

        var category = new Category
        {
            Name = request.Name,
            Id = NewId.NextSequentialGuid()
        };


        await context.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return ServiceResult<CreateCategoryResponse>.SuccessAsCreated(new CreateCategoryResponse(category.Id),
            "<empty>");
    }
}