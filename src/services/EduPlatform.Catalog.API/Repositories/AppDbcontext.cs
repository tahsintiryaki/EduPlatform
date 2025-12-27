using System.Reflection;
using EduPlatform.Catalog.API.Features.Categories;
using EduPlatform.Catalog.API.Features.Courses;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Catalog.API.Repositories;

public class AppDbcontext(DbContextOptions<AppDbcontext> options):DbContext(options)
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}