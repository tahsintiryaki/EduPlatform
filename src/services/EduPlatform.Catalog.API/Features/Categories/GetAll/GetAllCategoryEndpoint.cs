using AutoMapper;
using EduPlatform.Catalog.API.Features.Categories.Create;
using EduPlatform.Catalog.API.Repositories;
using EduPlatform.Shared;
using EduPlatform.Shared.Extensions;
using EduPlatform.Shared.Filters;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Catalog.API.Features.Categories.GetAll;

public class GetAllCategoryQuery : IRequest<ServiceResult<List<CategoryDto>>>;

public class GetAllCategoryQueryHandler(AppDbContext context,IMapper mapper)
    : IRequestHandler<GetAllCategoryQuery, ServiceResult<List<CategoryDto>>>
{
    public async Task<ServiceResult<List<CategoryDto>>> Handle(GetAllCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await context.Categories.ToListAsync(cancellationToken);
        var categoryDto = mapper.Map<List<CategoryDto>>(categories);
        return ServiceResult<List<CategoryDto>>.SuccessAsOk(categoryDto);
    }
}

public static class GetAllCategoryEndpoint
{
    public static RouteGroupBuilder GetAllCategoryGroupItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator mediator) =>
            (await mediator.Send(new GetAllCategoryQuery())).ToGenericResult());
        return group;
    }
}