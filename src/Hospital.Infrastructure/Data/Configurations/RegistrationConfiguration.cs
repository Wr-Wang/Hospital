using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.ToTable("Registrations", "opd");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.PatientId).IsRequired();
        builder.Property(r => r.ScheduleId).HasColumnName("SlotId").IsRequired();
        builder.Property(r => r.DoctorId).IsRequired();
        builder.Property(r => r.DeptId).IsRequired();
        builder.Property(r => r.CampusId).IsRequired();
        builder.Property(r => r.RegisterTime).HasColumnName("CreatedAt").IsRequired();

        builder.Property(r => r.QueueNumber).HasColumnName("QueueNo").HasDefaultValue(0);
        builder.Property(r => r.SlotName).HasMaxLength(64);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(64);
    }
}
