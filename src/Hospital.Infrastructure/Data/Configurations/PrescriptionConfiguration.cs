using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("Prescriptions", "pha");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.EncounterId)
            .HasColumnName("OutpatientEncounterId");

        builder.Property(p => p.DoctorId).HasColumnName("PrescribedByStaffId");

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(64);

        // PrescriptionItems collection
        builder.HasMany(p => p.Items)
            .WithOne()
            .HasForeignKey("PrescriptionId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
