using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Infrastructure.Data.Configurations;

public class WeChatAccountConfiguration : IEntityTypeConfiguration<WeChatAccount>
{
    public void Configure(EntityTypeBuilder<WeChatAccount> builder)
    {
        builder.ToTable("WeChatAccounts", "sec");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.OpenId).HasMaxLength(128).IsRequired();
        builder.HasIndex(w => w.OpenId).IsUnique();

        builder.Property(w => w.UnionId).HasMaxLength(128);
        builder.Property(w => w.NickName).HasMaxLength(100);
        builder.Property(w => w.AvatarUrl).HasMaxLength(500);
        builder.Property(w => w.Phone).HasMaxLength(32);

        builder.Property(w => w.CreatedAt).HasDefaultValueSql("GETDATE()");
        builder.Property(w => w.LastLoginAt).HasDefaultValueSql("GETDATE()");
    }
}
