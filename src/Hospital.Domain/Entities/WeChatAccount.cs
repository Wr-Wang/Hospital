namespace Hospital.Domain.Entities;

/// <summary>微信账号关联（患者）</summary>
public class WeChatAccount : Entity
{
    // EF Core
    private WeChatAccount() { }

    public WeChatAccount(string openId, long patientId, string? nickName = null)
    {
        OpenId = openId;
        PatientId = patientId;
        NickName = nickName;
        CreatedAt = DateTime.UtcNow;
        LastLoginAt = DateTime.UtcNow;
    }

    public string OpenId { get; private set; } = string.Empty;
    public string? UnionId { get; private set; }
    public long PatientId { get; private set; }
    public string? NickName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? Phone { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastLoginAt { get; private set; }

    public void UpdateLoginTime() => LastLoginAt = DateTime.UtcNow;

    public void UpdateProfile(string? nickName, string? avatarUrl)
    {
        if (nickName != null) NickName = nickName;
        if (avatarUrl != null) AvatarUrl = avatarUrl;
    }

    public void BindPhone(string phone) => Phone = phone;

    public void BindUnionId(string unionId) => UnionId = unionId;
}
