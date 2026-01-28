using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Payment.Outbox.Worker.Service;

public class PaymentOutboxReadDbContext:DbContext
{
    public PaymentOutboxReadDbContext(DbContextOptions<PaymentOutboxReadDbContext> options) : base(options)
    {
    }
    public DbSet<PaymentOutbox> PaymentOutboxes { get; set; }

    
}