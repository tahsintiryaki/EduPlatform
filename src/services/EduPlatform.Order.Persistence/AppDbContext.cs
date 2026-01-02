#region

using EduPlatform.Order.Domain.Entities;
using EduPlatform.Order.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

#endregion

namespace EduPlatform.Order.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Address> Addresses { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderConfiguration).Assembly);


        base.OnModelCreating(modelBuilder);
    }
}