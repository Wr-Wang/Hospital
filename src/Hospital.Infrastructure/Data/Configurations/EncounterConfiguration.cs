using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class EncounterConfiguration : IEntityTypeConfiguration<Encounter>
{
    public void Configure(EntityTypeBuilder<Encounter> builder)
    {
        builder.ToTable("OutpatientEncounters", "enc");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.PatientId).IsRequired();
        builder.Property(e => e.DoctorId).HasColumnName("StaffId");
        builder.Property(e => e.DeptId).HasColumnName("DepartmentId").IsRequired();
        builder.Property(e => e.CampusId).IsRequired();
        builder.Property(e => e.RegistrationId).IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(e => e.StartTime).HasColumnName("StartedAt");
        builder.Property(e => e.EndTime).HasColumnName("EndedAt");
    }
}
