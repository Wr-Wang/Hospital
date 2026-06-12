using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class BillingConfiguration : IEntityTypeConfiguration<Billing>
{
    public void Configure(EntityTypeBuilder<Billing> builder)
    {
        builder.ToTable("Invoices", "fin");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.PatientId).HasColumnName("PayerPatientId").IsRequired();
        builder.Property(b => b.PatientName).HasMaxLength(100);
        builder.Property(b => b.TotalAmount).HasColumnType("decimal(18,4)").HasDefaultValue(0);

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(b => b.PaidAt).HasColumnName("SettledAt");

        // Items collection
        builder.HasMany(b => b.Items)
            .WithOne()
            .HasForeignKey("BillingId")
            .OnDelete(DeleteBehavior.Cascade);

        // Payments collection
        builder.HasMany(b => b.Payments)
            .WithOne()
            .HasForeignKey("BillingId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
