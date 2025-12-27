using EduPlatform.Shared;
using MediatR;

namespace EduPlatform.Catalog.API.Features.Categories.Create;

public record CreateCategoryCommand(string Name) : IRequest<ServiceResult<CreateCategoryResponse>>
{
}