using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Payment.API.Repositories;

public class InboxDbContext:DbContext
{
    public InboxDbContext(DbContextOptions<InboxDbContext> options):base(options)
    {
        
    }

    public DbSet<OrderInbox> OrderInboxes { get; set; }
}