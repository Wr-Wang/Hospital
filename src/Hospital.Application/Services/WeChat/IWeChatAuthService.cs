using Hospital.Application.DTOs;

namespace Hospital.Application.Services.WeChat;

/// <summary>微信认证服务</summary>
public interface IWeChatAuthService
{
    /// <summary>code → openid → 已绑定则直接 JWT，否则返回临时 token</summary>
    Task<WeChatLoginResult> LoginAsync(WeChatLoginRequest request);

    /// <summary>创建新患者并绑定微信</summary>
    Task<WeChatAuthResult> CreatePatientAsync(string tempToken, string name, string? phone = null);

    /// <summary>refresh_token → 新 access_token</summary>
    Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>撤销 refresh_token</summary>
    Task LogoutAsync(long patientId, string refreshToken);

    /// <summary>获取当前登录患者的资料</summary>
    Task<PatientProfileResult> GetCurrentPatientAsync(long patientId);
}
