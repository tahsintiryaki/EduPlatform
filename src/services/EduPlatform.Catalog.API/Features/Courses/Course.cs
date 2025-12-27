using EduPlatform.Catalog.API.Features.Categories;
using EduPlatform.Catalog.API.Repositories;
using MongoDB.Driver.Core.Misc;

namespace EduPlatform.Catalog.API.Features.Courses;

public class Course : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public Guid UserId { get; set; }
    public string? Picture { get; set; }

    public DateTime Created { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public Feature Feature { get; set; } = null!;

}