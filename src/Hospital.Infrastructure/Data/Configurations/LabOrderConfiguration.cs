using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class LabOrderConfiguration : IEntityTypeConfiguration<LabOrder>
{
    public void Configure(EntityTypeBuilder<LabOrder> builder)
    {
        builder.ToTable("LabOrders", "lab");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.EncounterId)
            .HasColumnName("OutpatientEncounterId");

        builder.Property(o => o.ItemCode).HasMaxLength(64);
        builder.Property(o => o.ItemName).HasMaxLength(256);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(64);
    }
}
