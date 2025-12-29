using EduPlatform.Catalog.API.Repositories;

namespace EduPlatform.Catalog.API.Features.Categories.GetById;

public record GetCategoryByIdQuery(Guid Id) : IRequestByServiceResult<CategoryDto>;

public class GetCategoryByIdQueryHandler(AppDbContext context, IMapper _mapper)
    : IRequestHandler<GetCategoryByIdQuery, ServiceResult<CategoryDto>>
{
    public async Task<ServiceResult<CategoryDto>> Handle(GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await context.Categories.FindAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return ServiceResult<CategoryDto>.Error("Category Not Found", $"The category with '{request.Id}' not found",
                HttpStatusCode.NotFound);
        }

        var categoryDto = _mapper.Map<CategoryDto>(category);
        return ServiceResult<CategoryDto>.SuccessAsOk(categoryDto);
    }
}

public static class GetCategoryByIdEndpoint
{
    public static RouteGroupBuilder GetByIdCategoryGroupItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (IMediator mediator, Guid id) =>
            (await mediator.Send(new GetCategoryByIdQuery(id))).ToGenericResult())
            .MapToApiVersion(1, 0);
        return group;
    }
}