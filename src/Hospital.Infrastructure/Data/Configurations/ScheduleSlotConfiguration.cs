using Hospital.Domain.Aggregates.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class ScheduleSlotConfiguration : IEntityTypeConfiguration<ScheduleSlot>
{
    public void Configure(EntityTypeBuilder<ScheduleSlot> builder)
    {
        builder.ToTable("ScheduleSlots", "opd");
        builder.HasKey(s => s.Id);

        builder.OwnsOne(s => s.TimeSlot, ts =>
        {
            ts.Property(t => t.Name).HasColumnName("SlotType").HasMaxLength(64);
            ts.Property(t => t.StartTime).HasColumnName("StartTime");
            ts.Property(t => t.EndTime).HasColumnName("EndTime");
        });

        builder.Property(s => s.TotalQuota).HasDefaultValue(0);
        builder.Property(s => s.BookedQuota).HasDefaultValue(0);

        // Shadow property
        builder.Property<long>("TemplateId").IsRequired();
    }
}
