using Hospital.Domain.Aggregates.Patient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients", "pat");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PatientNo).HasMaxLength(64).IsRequired();

        builder.OwnsOne(p => p.IdCard, idCard =>
        {
            idCard.Property(i => i.Number)
                .HasColumnName("IdCardNo")
                .HasMaxLength(32);
        });

        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();

        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(16);

        builder.Property(p => p.BirthDate);
        builder.Property(p => p.AllergiesText).HasMaxLength(1000);

        builder.OwnsOne(p => p.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("Phone")
                .HasMaxLength(32);
        });

        // Identifiers collection
        builder.HasMany(p => p.Identifiers)
            .WithOne()
            .HasForeignKey("PatientId")
            .OnDelete(DeleteBehavior.Cascade);

        // Consents collection
        builder.HasMany(p => p.Consents)
            .WithOne()
            .HasForeignKey("PatientId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.PatientNo).IsUnique();
    }
}
