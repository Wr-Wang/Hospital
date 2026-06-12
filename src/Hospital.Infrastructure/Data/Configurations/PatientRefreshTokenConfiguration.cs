using Hospital.Infrastructure.ExternalServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

internal class PatientRefreshTokenConfiguration : IEntityTypeConfiguration<PatientRefreshToken>
{
    public void Configure(EntityTypeBuilder<PatientRefreshToken> builder)
    {
        builder.ToTable("PatientRefreshTokens", "sec");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token).HasMaxLength(500).IsRequired();
        builder.Property(t => t.ExpiresAt).IsRequired();
        builder.Property(t => t.CreatedAt).HasDefaultValueSql("GETDATE()");

        builder.HasIndex(t => t.PatientId);
    }
}
