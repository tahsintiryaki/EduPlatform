#region

using EduPlatform.Order.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#endregion

namespace EduPlatform.Order.Persistence.Configurations;

internal class OrderOutboxConfiguration : IEntityTypeConfiguration<Domain.Entities.OrderOutbox>
{
    
    public void Configure(EntityTypeBuilder<OrderOutbox> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("IdempotentToken");


    }
}