using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", "fin");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Method)
            .HasConversion<string>()
            .HasMaxLength(64)
            .HasColumnName("PayMethod");

        builder.Property(p => p.Amount).HasColumnType("decimal(18,4)");
        builder.Property(p => p.Remark).HasColumnName("TransactionRef").HasMaxLength(200);
        builder.Property(p => p.PaidAt).IsRequired();

        // Shadow property
        builder.Property<long>("BillingId").IsRequired();
    }
}
