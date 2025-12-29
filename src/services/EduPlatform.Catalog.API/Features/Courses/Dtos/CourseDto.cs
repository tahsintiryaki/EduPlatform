using EduPlatform.Catalog.API.Features.Categories;

namespace EduPlatform.Catalog.API.Features.Courses.Dtos;

public record CourseDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    DateTime Created,
    CategoryDto Category,
    FeatureDto Feature);