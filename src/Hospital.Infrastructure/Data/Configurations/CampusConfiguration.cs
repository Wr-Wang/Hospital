using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class CampusConfiguration : IEntityTypeConfiguration<Campus>
{
    public void Configure(EntityTypeBuilder<Campus> builder)
    {
        builder.ToTable("Campuses", "mdm");
        builder.HasKey(c => c.Id);

        builder.OwnsOne(c => c.Code, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("Code")
                .HasMaxLength(64)
                .IsRequired();
        });

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.Phone).HasMaxLength(32);
        builder.Property(c => c.IsActive).HasDefaultValue(true);
    }
}
