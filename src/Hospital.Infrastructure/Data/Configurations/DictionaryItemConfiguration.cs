using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class DictionaryItemConfiguration : IEntityTypeConfiguration<DictionaryItem>
{
    public void Configure(EntityTypeBuilder<DictionaryItem> builder)
    {
        builder.ToTable("DictionaryItems", "mdm");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.TypeId).IsRequired();
        builder.Property(d => d.Code).HasMaxLength(64).IsRequired();
        builder.Property(d => d.Name).HasMaxLength(200).IsRequired();
        builder.Property(d => d.SortOrder).HasDefaultValue(0);
        builder.Property(d => d.IsActive).HasDefaultValue(true);
        builder.Property(d => d.ParentId);

        // Navigation to DictionaryType
        builder.HasOne(d => d.Type)
            .WithMany()
            .HasForeignKey(d => d.TypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(d => new { d.TypeId, d.Code }).IsUnique();
    }
}
