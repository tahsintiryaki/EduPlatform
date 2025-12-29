using EduPlatform.Shared.Extensions;
using EduPlatform.Shared.Filters;

namespace EduPlatform.Catalog.API.Features.Categories.Create;

public static class CreateCategoryEndpoint
{
    public static RouteGroupBuilder CreateCategoryGroupItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateCategoryCommand command, IMediator mediator) =>
            (await mediator.Send(command)).ToGenericResult())
            .MapToApiVersion(1, 0)
            .AddEndpointFilter<ValidationFilter<CreateCategoryCommand>>();
        return group;
    }
}