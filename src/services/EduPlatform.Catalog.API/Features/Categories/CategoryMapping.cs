namespace EduPlatform.Catalog.API.Features.Categories;

public class CategoryMapping:Profile
{
    public CategoryMapping()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
    }
}