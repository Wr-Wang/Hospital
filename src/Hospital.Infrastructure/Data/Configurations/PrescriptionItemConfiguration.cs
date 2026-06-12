using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
{
    public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
    {
        builder.ToTable("PrescriptionLines", "pha");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.DrugName).HasMaxLength(256);
        builder.Property(i => i.Spec).HasMaxLength(128);
        builder.Property(i => i.Form).HasMaxLength(64);
        builder.Property(i => i.Freq).HasColumnName("Frequency").HasMaxLength(64);
        builder.Property(i => i.Dosage).HasColumnName("Dose").HasMaxLength(64);
        builder.Property(i => i.Duration).HasColumnName("Days");
        builder.Property(i => i.Qty).HasColumnType("decimal(18,4)");

        // Shadow property
        builder.Property<long>("PrescriptionId").IsRequired();
    }
}
