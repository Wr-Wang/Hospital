using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class DiagnosisConfiguration : IEntityTypeConfiguration<Diagnosis>
{
    public void Configure(EntityTypeBuilder<Diagnosis> builder)
    {
        builder.ToTable("Diagnoses", "enc");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.EncounterId)
            .HasColumnName("OutpatientEncounterId")
            .IsRequired();

        builder.Property(d => d.DiagnosisType)
            .HasConversion<string>()
            .HasMaxLength(64)
            .HasColumnName("DiagnosisType");

        builder.Property(d => d.IcdCode).HasMaxLength(32).IsRequired();
        builder.Property(d => d.Description).HasColumnName("IcdName").HasMaxLength(256).IsRequired();
        builder.Property(d => d.IsPrimary).HasDefaultValue(false);
    }
}
