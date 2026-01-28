using EduPlatform.Order.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduPlatform.Order.Persistence.Configurations;

public class PaymentInboxConfiguration : IEntityTypeConfiguration<PaymentInbox>
{
    public void Configure(EntityTypeBuilder<PaymentInbox> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("IdempotentToken");
    }
}