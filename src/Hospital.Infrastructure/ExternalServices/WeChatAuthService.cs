using Hospital.Application.DTOs;
using Hospital.Application.Services.WeChat;
using Hospital.Domain.Aggregates.Patient;
using Hospital.Domain.Entities;
using Hospital.Domain.ValueObjects;
using Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Hospital.Infrastructure.ExternalServices;

public sealed class WeChatAuthService : IWeChatAuthService
{
    private readonly WeChatHttpClient _weChatClient;
    private readonly JwtTokenService _jwtService;
    private readonly PatientNoService _patientNoService;
    private readonly HospitalDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<WeChatAuthService> _logger;

    public WeChatAuthService(
        WeChatHttpClient weChatClient,
        JwtTokenService jwtService,
        PatientNoService patientNoService,
        HospitalDbContext db,
        IMemoryCache cache,
        ILogger<WeChatAuthService> logger)
    {
        _weChatClient = weChatClient;
        _jwtService = jwtService;
        _patientNoService = patientNoService;
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>code → openid → 已绑定则直接签发 JWT，否则返回临时 token</summary>
    public async Task<WeChatLoginResult> LoginAsync(WeChatLoginRequest request)
    {
        var session = await _weChatClient.Code2SessionAsync(request.Code);
        var openId = session.Openid!;

        // 检查是否已绑定微信账号
        var existingAccount = await _db.Set<WeChatAccount>()
            .FirstOrDefaultAsync(w => w.OpenId == openId);

        if (existingAccount is not null)
        {
            existingAccount.UpdateLoginTime();
            var (accessToken, refreshToken) = await IssueTokenAsync(existingAccount.PatientId);

            var patient = await _db.Patients.FindAsync(existingAccount.PatientId);
            var patientNo = patient?.PatientNo ?? string.Empty;
            var name = patient?.Name ?? string.Empty;
            var phone = patient?.Phone?.Value ?? string.Empty;

            return new WeChatLoginResult(null, null, accessToken, refreshToken, existingAccount.PatientId, patientNo, name, phone, false);
        }

        // 未绑定：缓存 openid，返回临时 token
        var tempToken = Guid.NewGuid().ToString("N");
        _cache.Set(tempToken, openId, TimeSpan.FromMinutes(5));

        return new WeChatLoginResult(tempToken, 300, null, null, null, null, null, null, true);
    }

    /// <summary>创建新患者（手机号可选）并绑定微信</summary>
    public async Task<WeChatAuthResult> CreatePatientAsync(string tempToken, string name, string? phone = null)
    {
        var openId = _cache.Get<string>(tempToken)
            ?? throw new InvalidOperationException("临时 token 已过期，请重新登录");

        var patientNo = await _patientNoService.NextNoAsync();
        var phoneNumber = phone != null ? new PhoneNumber(phone) : null;
        var patient = new Patient(patientNo, name, null, null, phoneNumber, null, null);

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        await BindWeChatAccountAsync(openId, patient.Id, null);
        var (accessToken, refreshToken) = await IssueTokenAsync(patient.Id);

        return new WeChatAuthResult(accessToken, refreshToken, patient.Id, patient.PatientNo, patient.Name, phone, true);
    }

    /// <summary>获取当前患者资料</summary>
    public async Task<PatientProfileResult> GetCurrentPatientAsync(long patientId)
    {
        var patient = await _db.Patients.FindAsync(patientId)
            ?? throw new InvalidOperationException("患者不存在");

        return new PatientProfileResult(
            patient.Id,
            patient.PatientNo,
            patient.Name,
            patient.Phone?.Value
        );
    }

    /// <summary>刷新 access_token</summary>
    public async Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var tokenEntity = await _db.Set<PatientRefreshToken>()
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.RevokedAt == null);

        if (tokenEntity is null || tokenEntity.ExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("refresh_token 无效或已过期");

        tokenEntity.RevokedAt = DateTime.UtcNow;

        var (accessToken, refreshToken) = await IssueTokenAsync(tokenEntity.PatientId);

        return new RefreshTokenResult(accessToken, refreshToken);
    }

    /// <summary>撤销 refresh_token</summary>
    public async Task LogoutAsync(long patientId, string refreshToken)
    {
        var tokenEntity = await _db.Set<PatientRefreshToken>()
            .FirstOrDefaultAsync(t => t.PatientId == patientId && t.Token == refreshToken && t.RevokedAt == null);

        if (tokenEntity is not null)
        {
            tokenEntity.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    // ===== 内部方法 =====

    private async Task BindWeChatAccountAsync(string openId, long patientId, string? nickName)
    {
        var existing = await _db.Set<WeChatAccount>()
            .FirstOrDefaultAsync(w => w.OpenId == openId);

        if (existing is not null)
        {
            existing.UpdateLoginTime();
            if (existing.PatientId != patientId)
                throw new InvalidOperationException("该微信已绑定其他患者账号");
        }
        else
        {
            _db.Set<WeChatAccount>().Add(new WeChatAccount(openId, patientId, nickName));
        }

        await _db.SaveChangesAsync();
    }

    private async Task<(string accessToken, string refreshToken)> IssueTokenAsync(long patientId)
    {
        var accessToken = _jwtService.GeneratePatientToken(patientId, string.Empty);
        var refreshToken = _jwtService.GenerateRefreshToken();

        _db.Set<PatientRefreshToken>().Add(new PatientRefreshToken
        {
            PatientId = patientId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return (accessToken, refreshToken);
    }
}
