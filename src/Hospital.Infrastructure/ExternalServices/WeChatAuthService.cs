using Hospital.Application.DTOs;
using Hospital.Application.Services.WeChat;
using Hospital.Domain.Aggregates.Patient;
using Hospital.Domain.Entities;
using Hospital.Domain.ValueObjects;
using Hospital.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    private readonly int _tempTokenExpiryMinutes;
    private readonly int _refreshTokenExpiryDays;

    public WeChatAuthService(
        WeChatHttpClient weChatClient,
        [FromKeyedServices("patient")] JwtTokenService jwtService,
        PatientNoService patientNoService,
        HospitalDbContext db,
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<WeChatAuthService> logger)
    {
        _weChatClient = weChatClient;
        _jwtService = jwtService;
        _patientNoService = patientNoService;
        _db = db;
        _cache = cache;
        _logger = logger;
        _tempTokenExpiryMinutes = int.Parse(configuration["WeChat:Jwt:TempTokenExpiryMinutes"] ?? "5");
        _refreshTokenExpiryDays = int.Parse(configuration["WeChat:Jwt:RefreshTokenExpiryDays"] ?? "30");
    }

    /// <summary>code → openid → 已绑定则直接签发 JWT，否则返回临时 token</summary>
    public async Task<WeChatLoginResult> LoginAsync(WeChatLoginRequest request)
    {
        _logger.LogInformation("步骤1: 收到微信登录请求，开始换取 openid");
        var session = await _weChatClient.Code2SessionAsync(request.Code);
        var openId = session.Openid!;
        _logger.LogInformation("步骤2: code2session 成功，openid={OpenId}", openId);

        // 检查是否已绑定微信账号
        _logger.LogInformation("步骤3: 查询微信账号绑定关系");
        var existingAccount = await _db.Set<WeChatAccount>()
            .FirstOrDefaultAsync(w => w.OpenId == openId);

        if (existingAccount is not null)
        {
            _logger.LogInformation("步骤3.1: 已绑定，patientId={PatientId}", existingAccount.PatientId);
            existingAccount.UpdateLoginTime();

            var patient = await _db.Patients.FindAsync(existingAccount.PatientId);
            var patientNo = patient?.PatientNo ?? string.Empty;
            var name = patient?.Name ?? string.Empty;
            var phone = patient?.Phone?.Value ?? string.Empty;

            _logger.LogInformation("步骤4: 签发 JWT（已绑定用户）");
            var (accessToken, refreshToken) = await IssueTokenAsync(existingAccount.PatientId, name);

            return new WeChatLoginResult(null, null, accessToken, refreshToken, existingAccount.PatientId, patientNo, name, phone, false);
        }

        // 未绑定：缓存 openid，返回临时 token
        _logger.LogInformation("步骤3.2: 未绑定微信，生成临时 token");
        var tempToken = Guid.NewGuid().ToString("N");
        _cache.Set(tempToken, openId, TimeSpan.FromMinutes(_tempTokenExpiryMinutes));

        return new WeChatLoginResult(tempToken, _tempTokenExpiryMinutes * 60, null, null, null, null, null, null, true);
    }

    /// <summary>创建新患者（手机号可选）并绑定微信（幂等：同一 openid 重复建档直接返回已有患者）</summary>
    public async Task<WeChatAuthResult> CreatePatientAsync(string tempToken, string name, string? phone = null)
    {
        var openId = _cache.Get<string>(tempToken)
            ?? throw new InvalidOperationException("临时 token 已过期，请重新登录");
        _cache.Remove(tempToken); // 临时 token 一次性使用：防止重复提交建档

        // 幂等：该微信已绑定过患者 → 直接返回已有患者
        var existing = await _db.Set<WeChatAccount>()
            .FirstOrDefaultAsync(w => w.OpenId == openId);
        if (existing is not null)
            return await IssueForBoundAsync(existing);

        try
        {
            // 患者 + 微信绑定在同一事务中：任一步失败整体回滚，避免留下孤儿患者
            await using var tx = await _db.Database.BeginTransactionAsync();

            var patientNo = await _patientNoService.NextNoAsync();
            var phoneNumber = phone != null ? new PhoneNumber(phone) : null;
            var patient = new Patient(patientNo, name, null, null, phoneNumber, null, null);

            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            await BindWeChatAccountAsync(openId, patient.Id, null);

            await tx.CommitAsync();

            var (accessToken, refreshToken) = await IssueTokenAsync(patient.Id, patient.Name);
            return new WeChatAuthResult(accessToken, refreshToken, patient.Id, patient.PatientNo, patient.Name, phone, true);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyError(ex))
        {
            // 并发重复提交：另一请求已为同一 openid 建档，本事务已回滚，返回已绑定患者
            _db.ChangeTracker.Clear(); // 丢弃本次失败事务的跟踪状态，避免后续 SaveChanges 重放
            var bound = await _db.Set<WeChatAccount>()
                .FirstOrDefaultAsync(w => w.OpenId == openId);
            if (bound is not null)
                return await IssueForBoundAsync(bound);
            throw;
        }
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

        var patient = await _db.Patients.FindAsync(tokenEntity.PatientId);
        var name = patient?.Name ?? string.Empty;
        var (accessToken, refreshToken) = await IssueTokenAsync(tokenEntity.PatientId, name);

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

    /// <summary>openid 已绑定时：刷新登录时间并签发 JWT（IsNew=false）</summary>
    private async Task<WeChatAuthResult> IssueForBoundAsync(WeChatAccount account)
    {
        account.UpdateLoginTime();

        var patient = await _db.Patients.FindAsync(account.PatientId)
            ?? throw new InvalidOperationException("绑定患者不存在");

        var (accessToken, refreshToken) = await IssueTokenAsync(patient.Id, patient.Name);
        return new WeChatAuthResult(accessToken, refreshToken, patient.Id, patient.PatientNo, patient.Name, patient.Phone?.Value, false);
    }

    /// <summary>是否唯一索引/唯一约束冲突（SQL Server 错误 2601/2627）</summary>
    private static bool IsDuplicateKeyError(DbUpdateException ex) =>
        ex.InnerException is SqlException { Number: 2601 or 2627 };

    private async Task<(string accessToken, string refreshToken)> IssueTokenAsync(long patientId, string name)
    {
        var accessToken = _jwtService.GeneratePatientToken(patientId, name);
        var refreshToken = _jwtService.GenerateRefreshToken();

        _db.Set<PatientRefreshToken>().Add(new PatientRefreshToken
        {
            PatientId = patientId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return (accessToken, refreshToken);
    }
}
