using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class DrugInventoryConfiguration : IEntityTypeConfiguration<DrugInventory>
{
    public void Configure(EntityTypeBuilder<DrugInventory> builder)
    {
        builder.ToTable("DrugBatches", "pha");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.DrugCode).HasMaxLength(50);
        builder.Property(d => d.DrugName).HasMaxLength(200);
        builder.Property(d => d.Spec).HasMaxLength(100);
        builder.Property(d => d.BatchNo).HasMaxLength(100);
        builder.Property(d => d.ExpiryDate);
        builder.Property(d => d.TotalQuantity).HasColumnType("decimal(18,4)");
        builder.Property(d => d.AvailableQuantity).HasColumnType("decimal(18,4)");
    }
}
