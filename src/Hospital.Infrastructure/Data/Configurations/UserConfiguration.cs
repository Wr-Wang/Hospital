using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "sec");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.LoginName).HasMaxLength(128).IsRequired();
        builder.Property(u => u.Password).HasColumnName("PasswordHash").HasMaxLength(500).IsRequired();
        builder.Property(u => u.DisplayName).HasMaxLength(100);
        builder.Property(u => u.CampusName).HasMaxLength(50);

        builder.Property(u => u.IsActive)
            .HasColumnName("IsLocked")
            .HasConversion(v => !v, v => !v);

        builder.Ignore(u => u.Roles);
    }
}
