using Hospital.Domain.Aggregates.Patient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class PatientConsentConfiguration : IEntityTypeConfiguration<PatientConsent>
{
    public void Configure(EntityTypeBuilder<PatientConsent> builder)
    {
        builder.ToTable("PatientConsents", "pat");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ConsentType).HasMaxLength(128).IsRequired();
        builder.Property(c => c.GrantedAt).IsRequired();
        builder.Property(c => c.ExpiresAt);
        builder.Property(c => c.DocumentRef).HasMaxLength(500);

        // Shadow property for the FK
        builder.Property<long>("PatientId").IsRequired();
    }
}
