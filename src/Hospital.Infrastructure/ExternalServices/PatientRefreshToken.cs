namespace Hospital.Infrastructure.ExternalServices;

/// <summary>
/// 患者 RefreshToken 实体（仅用于 Infrastructure 层，非 Domain 实体）。
/// 用于持久化 refresh_token，支持撤销和过期管理。
/// </summary>
internal sealed class PatientRefreshToken
{
    public long Id { get; set; }
    public long PatientId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
