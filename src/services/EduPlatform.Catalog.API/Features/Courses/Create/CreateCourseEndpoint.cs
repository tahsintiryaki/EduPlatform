using EduPlatform.Catalog.API.Features.Categories.Create;
using EduPlatform.Shared.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Catalog.API.Features.Courses.Create;

public static class CreateCourseEndpoint
{
    public static RouteGroupBuilder CreateCourseGroupItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async ([FromForm]CreateCourseCommand command, IMediator mediator) =>
                (await mediator.Send(command)).ToGenericResult())
            .WithName("CreateCourse")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<ValidationFilter<CreateCourseCommand>>()
            .MapToApiVersion(1, 0).DisableAntiforgery()
            .RequireAuthorization(policyNames: "InstructorPolicy");;
        return group;
    }
}