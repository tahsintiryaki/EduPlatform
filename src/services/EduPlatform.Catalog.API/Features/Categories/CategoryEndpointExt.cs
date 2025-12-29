using Asp.Versioning.Builder;
using EduPlatform.Catalog.API.Features.Categories.Create;
using EduPlatform.Catalog.API.Features.Categories.GetAll;
using EduPlatform.Catalog.API.Features.Categories.GetById;

namespace EduPlatform.Catalog.API.Features.Categories;

public static class CategoryEndpointExt
{
    public static void AddCategoryGroupEndpointExt(this WebApplication app,ApiVersionSet apiVersionSet)
    {
        app.MapGroup("api/v1/categories").WithTags("Category")
            .WithApiVersionSet(apiVersionSet)
            .CreateCategoryGroupItemEndpoint()
            .GetAllCategoryGroupItemEndpoint()
            .GetByIdCategoryGroupItemEndpoint();
    }
}