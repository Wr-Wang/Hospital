using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class DispensingConfiguration : IEntityTypeConfiguration<Dispensing>
{
    public void Configure(EntityTypeBuilder<Dispensing> builder)
    {
        builder.ToTable("Dispenses", "pha");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.PrescriptionId).IsRequired();
        builder.Property(d => d.DispensedBy).IsRequired();
        builder.Property(d => d.OperatedAt).HasColumnName("CreatedAt").IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(d => d.Remark).HasMaxLength(500);

        // Items collection
        builder.HasMany(d => d.Items)
            .WithOne()
            .HasForeignKey("DispensingId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
