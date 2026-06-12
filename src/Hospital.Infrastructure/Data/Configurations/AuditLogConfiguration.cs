using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "sec");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.UserName).HasMaxLength(50);
        builder.Property(a => a.Action).HasMaxLength(128).IsRequired();
        builder.Property(a => a.EntityType).HasMaxLength(128).IsRequired();
        builder.Property(a => a.EntityId).HasColumnName("EntityId").HasMaxLength(128);
        builder.Property(a => a.OldValue).HasColumnName("DetailJson");
        builder.Property(a => a.NewValue);
        builder.Property(a => a.IpAddress).HasMaxLength(64);
        builder.Property(a => a.Timestamp).HasColumnName("OccurredAt").IsRequired();
    }
}
