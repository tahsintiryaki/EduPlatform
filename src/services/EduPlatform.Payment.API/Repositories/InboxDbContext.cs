using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Payment.API.Repositories;

public class InboxDbContext : DbContext
{
    public InboxDbContext(DbContextOptions<InboxDbContext> options) : base(options)
    {
    }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<OrderInbox> OrderInboxes { get; set; }
    public DbSet<PaymentOutbox> PaymentOutboxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PaymentOutbox>().HasKey(t => t.IdempotentToken);
        
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever(); // Use sequential GUIDs
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.OrderCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Created).IsRequired();
            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired();
        });
    }
}