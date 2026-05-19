using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments", "mdm");
        builder.HasKey(d => d.Id);

        builder.OwnsOne(d => d.Code, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("Code")
                .HasMaxLength(64)
                .IsRequired();
        });

        builder.Property(d => d.Name).HasMaxLength(200).IsRequired();
        builder.Property(d => d.CampusId).IsRequired();
        builder.Property(d => d.ParentId);
        builder.Property(d => d.IsActive).HasDefaultValue(true);

        builder.Property(d => d.Type)
            .HasConversion<string>()
            .HasMaxLength(64)
            .HasColumnName("DeptType");

        // Self-referencing relationship
        builder.HasOne(d => d.Parent)
            .WithMany(d => d.Children)
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(d => new { d.CampusId, d.Code }).IsUnique();
    }
}
