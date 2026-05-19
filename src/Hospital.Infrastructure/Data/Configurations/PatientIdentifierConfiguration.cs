using Hospital.Domain.Aggregates.Patient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class PatientIdentifierConfiguration : IEntityTypeConfiguration<PatientIdentifier>
{
    public void Configure(EntityTypeBuilder<PatientIdentifier> builder)
    {
        builder.ToTable("PatientIdentifiers", "pat");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.IdType).HasMaxLength(64).IsRequired();
        builder.Property(i => i.IdValue).HasMaxLength(128).IsRequired();
        builder.Property(i => i.IsPrimary).HasDefaultValue(false);

        // Shadow property for the FK
        builder.Property<long>("PatientId").IsRequired();
    }
}
