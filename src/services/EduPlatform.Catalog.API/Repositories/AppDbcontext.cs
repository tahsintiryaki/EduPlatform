using System.Reflection;
using EduPlatform.Catalog.API.Features.Categories;
using EduPlatform.Catalog.API.Features.Courses;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace EduPlatform.Catalog.API.Repositories;

public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    public static AppDbContext Create(IMongoDatabase database)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<AppDbContext>().UseMongoDB(database.Client,
                database.DatabaseNamespace.DatabaseName);
        
        return new AppDbContext(optionsBuilder.Options);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}