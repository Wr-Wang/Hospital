using Hospital.Domain.Aggregates.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("ScheduleTemplates", "opd");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.DoctorId).IsRequired();
        builder.Property(s => s.DeptId).HasColumnName("DepartmentId").IsRequired();
        builder.Property(s => s.CampusId).IsRequired();
        builder.Property(s => s.ScheduleDate);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(64);

        // ScheduleSlots collection
        builder.HasMany(s => s.Slots)
            .WithOne()
            .HasForeignKey("TemplateId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
