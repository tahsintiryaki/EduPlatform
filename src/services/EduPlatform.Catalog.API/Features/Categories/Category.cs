using EduPlatform.Catalog.API.Features.Courses;
using EduPlatform.Catalog.API.Repositories;

namespace EduPlatform.Catalog.API.Features.Categories;

public class Category:BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<Course>? Courses { get; set; }
}