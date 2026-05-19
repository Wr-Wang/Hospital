using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class DispenseItemConfiguration : IEntityTypeConfiguration<DispenseItem>
{
    public void Configure(EntityTypeBuilder<DispenseItem> builder)
    {
        builder.ToTable("DispenseLines", "pha");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.DrugInventoryId).HasColumnName("InventoryLotId");
        builder.Property(i => i.DrugName).HasMaxLength(256);
        builder.Property(i => i.Spec).HasMaxLength(128);
        builder.Property(i => i.Quantity).HasColumnName("Qty").HasColumnType("decimal(18,4)");

        builder.Property<long>("DispensingId").IsRequired();
    }
}
