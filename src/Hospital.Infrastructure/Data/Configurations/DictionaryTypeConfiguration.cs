using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class DictionaryTypeConfiguration : IEntityTypeConfiguration<DictionaryType>
{
    public void Configure(EntityTypeBuilder<DictionaryType> builder)
    {
        builder.ToTable("DictionaryTypes", "mdm");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Code).HasMaxLength(64).IsRequired();
        builder.Property(d => d.Name).HasMaxLength(200).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.IsActive).HasDefaultValue(true);

        builder.HasIndex(d => d.Code).IsUnique();
    }
}
