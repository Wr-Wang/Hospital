using Hospital.Domain.Entities;
using Hospital.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff", "mdm");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .HasColumnName("EmployeeNo")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(s => s.Name)
            .HasColumnName("FullName")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Gender)
            .HasConversion<string>()
            .HasMaxLength(16);

        builder.Property(s => s.Phone).HasMaxLength(32);

        builder.Property(s => s.CampusId).IsRequired();
        builder.Property(s => s.DeptId).IsRequired();

        builder.Property(s => s.LicenseType)
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(s => s.LicenseNo)
            .HasColumnName("LicenseNo")
            .HasConversion(ValueConverters.LicenseNumberConverter)
            .HasMaxLength(128);

        builder.Property(s => s.LicenseExpiry)
            .HasColumnName("LicenseExpireDate");
        builder.Property(s => s.IsActive).HasDefaultValue(true);
    }
}
