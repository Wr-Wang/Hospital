using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class BillingItemConfiguration : IEntityTypeConfiguration<BillingItem>
{
    public void Configure(EntityTypeBuilder<BillingItem> builder)
    {
        builder.ToTable("ChargeLines", "fin");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ItemType).HasMaxLength(64);
        builder.Property(i => i.ItemName).HasMaxLength(256);
        builder.Property(i => i.Amount).HasColumnType("decimal(18,4)");

        // Shadow property
        builder.Property<long>("BillingId").IsRequired();
    }
}
