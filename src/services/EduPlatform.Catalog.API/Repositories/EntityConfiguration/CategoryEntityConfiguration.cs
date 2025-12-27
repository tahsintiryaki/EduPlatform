using EduPlatform.Catalog.API.Features.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;

namespace EduPlatform.Catalog.API.Repositories.EntityConfiguration;

public class CategoryEntityConfiguration:IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToCollection("categories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        //navigation prop ignored 
        builder.Ignore(x => x.Courses);
    }
}