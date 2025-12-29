using EduPlatform.Catalog.API.Features.Categories.Create;
using EduPlatform.Catalog.API.Features.Categories.GetAll;
using EduPlatform.Catalog.API.Features.Categories.GetById;

namespace EduPlatform.Catalog.API.Features.Categories;

public static class CategoryEndpointExt
{
    public static void AddCategoryGroupEndpointExt(this WebApplication app)
    {
        app.MapGroup("api/categories")
            .CreateCategoryGroupItemEndpoint()
            .GetAllCategoryGroupItemEndpoint()
            .GetByIdCategoryGroupItemEndpoint();
    }
}