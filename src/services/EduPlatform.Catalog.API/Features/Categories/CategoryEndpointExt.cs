using EduPlatform.Catalog.API.Features.Categories.Create;

namespace EduPlatform.Catalog.API.Features.Categories;

public static class CategoryEndpointExt
{
    public static void AddCategoryGroupEndpointExt(this WebApplication app)
    {
        app.MapGroup("api/categories").CreateCategoryGroupItemEndpoint();
    }
}