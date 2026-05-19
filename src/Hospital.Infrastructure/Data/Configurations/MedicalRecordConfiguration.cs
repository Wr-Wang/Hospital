using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.ToTable("EmrDocuments", "enc");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.EncounterId)
            .HasColumnName("OutpatientEncounterId");

        builder.Property(r => r.ChiefComplaint)
            .HasColumnName("ContentJson")
            .HasMaxLength(4000);

        builder.Ignore(r => r.PresentIllness);
        builder.Ignore(r => r.PastHistory);
        builder.Ignore(r => r.PhysicalExam);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(64)
            .HasColumnName("DocType");

        builder.Property(r => r.Version).HasDefaultValue(1);
    }
}
