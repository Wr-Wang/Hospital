# 微信登录功能 — 实现设计

> 基于 [WECHAT_LOGIN_REQUIREMENTS.md](WECHAT_LOGIN_REQUIREMENTS.md) 的实现方案。
> 分层架构：Domain → Application → Infrastructure → Api + 小程序前端。

---

## 总览：文件改动清单

### 新增 13 个文件

| # | 层 | 文件路径 | 用途 |
|---|-----|---------|------|
| 1 | Domain | `src/Hospital.Domain/Entities/WeChatAccount.cs` | 微信账号实体 |
| 2 | Domain | `src/Hospital.Domain/ValueObjects/WeChatOpenId.cs` | openid 值对象 |
| 3 | Application | `src/Hospital.Application/DTOs/WeChatAuthDTOs.cs` | 微信登录相关 DTO |
| 4 | Application | `src/Hospital.Application/Services/WeChat/IWeChatAuthService.cs` | 微信认证应用服务接口 |
| 5 | Infrastructure | `src/Hospital.Infrastructure/Data/Configurations/WeChatAccountConfiguration.cs` | EF Core 配置 |
| 6 | Infrastructure | `src/Hospital.Infrastructure/Data/Configurations/PatientRefreshTokenConfiguration.cs` | EF Core 配置 |
| 7 | Infrastructure | `src/Hospital.Infrastructure/ExternalServices/WeChatHttpClient.cs` | 微信 API 客户端 |
| 8 | Infrastructure | `src/Hospital.Infrastructure/ExternalServices/WeChatAuthService.cs` | 微信认证实现 |
| 9 | Infrastructure | `src/Hospital.Infrastructure/ExternalServices/PatientJwtService.cs` | 患者 JWT 生成 |
| 10 | Infrastructure | `src/Hospital.Infrastructure/ExternalServices/PatientNoService.cs` | PatientNo 编号生成 |
| 11 | Api | `src/Hospital.Api/Controllers/MiniProgramAuthController.cs` | 微信登录 API |
| 12 | Database | `database/010_wechat_login.sql` | 数据库迁移脚本 |
| 13 | Miniapp | `hospital-miniapp/services/wechat-auth-service.js` | 微信登录服务 |

### 修改 6 个文件

| # | 层 | 文件路径 | 改动内容 |
|---|-----|---------|---------|
| 1 | Infrastructure | `src/Hospital.Infrastructure/Data/HospitalDbContext.cs` | 新增 `WeChatAccounts` 和 `PatientRefreshTokens` 两个 DbSet |
| 2 | Api | `src/Hospital.Api/Program.cs` | 注册微信相关服务（5 行） |
| 3 | Api | `src/Hospital.Api/appsettings.json` | 新增 WeChat 配置节 |
| 4 | Miniapp | `hospital-miniapp/pages/login/login.js` | 重构登录页（仅微信登录，去掉账号密码） |
| 5 | Miniapp | `hospital-miniapp/utils/api.js` | 增加 401 自动刷新拦截 |
| 6 | Miniapp | `hospital-miniapp/app.js` | 增加 refresh_token 存取 |

---

## 两套认证体系全景图

新增的患者微信登录与现有后台账号密码登录完全隔离，各用各的表、各用各的 JWT、各用各的 Controller。

```
                       患者体系（新增）                         工作人员体系（现有）
                     ────────────────                       ─────────────────────
认证方式             微信一键登录                              用户名 + 密码

Domain 实体          WeChatAccount（新增）                    User（已有）
                    Patient（已有）                          Role（已有）

数据库表             sec.WeChatAccounts（新增）               sec.Users（已有）
                    sec.PatientRefreshTokens（新增）         sec.Roles（已有）
                    pat.Patients（已有）

JWT 服务             PatientJwtService（新增）                JwtTokenService（已有）
                    独立 SecretKey + Issuer                  独立 SecretKey + Issuer

认证方案名           "PatientJwt"（新增）                    默认 (JwtBearerDefaults)

JWT 载荷主体         patient:{patientId}                     userId / staffId

API Controller      MiniProgramAuthController（新增）        AuthenticationController（已有）
                    路由: api/miniprogram/auth/*             路由: api/authentication/*

客户端              微信小程序（患者专用）                     WPF 桌面端 + Web 管理端

DI 注册              AddHttpClient<WeChatHttpClient>()       AddSingleton<JwtTokenService>()
                    AddSingleton<PatientJwtService>()        AddScoped<LocalAuthenticationService>()
                    AddScoped<IWeChatAuthService>()

配置节              WeChat:MiniProgram / WeChat:Jwt          JwtSettings
```

**关键规则**：
- 患者 JWT 无法调用标注 `[Authorize]`（默认方案）的 Controller，只能调用标注 `[Authorize(AuthenticationSchemes = "PatientJwt")]` 的接口
- 后台 JWT 无法调用标注 `"PatientJwt"` 的接口
- 数据库中患者表（`pat.*` / `sec.WeChatAccounts`）与工作人员表（`sec.Users` / `sec.Roles`）无任何外键关联
- 小程序只对接患者体系，WPF 和 Web 管理端只对接工作人员体系

### 1.1 新增 `WeChatAccount` 实体

```
src/Hospital.Domain/Entities/WeChatAccount.cs
```

```csharp
namespace Hospital.Domain.Entities;

public class WeChatAccount : Entity
{
    public string OpenId { get; private set; }
    public string? UnionId { get; private set; }
    public long PatientId { get; private set; }
    public string? NickName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? Phone { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastLoginAt { get; private set; }

    // EF Core 私有构造
    private WeChatAccount() { }

    public WeChatAccount(string openId, long patientId, string? nickName = null)
    {
        OpenId = openId;
        PatientId = patientId;
        NickName = nickName;
        CreatedAt = DateTime.UtcNow;
        LastLoginAt = DateTime.UtcNow;
    }

    public void UpdateLoginTime() => LastLoginAt = DateTime.UtcNow;
    public void UpdateProfile(string? nickName, string? avatarUrl)
    {
        if (nickName != null) NickName = nickName;
        if (avatarUrl != null) AvatarUrl = avatarUrl;
    }
    public void BindPhone(string phone) => Phone = phone;
}
```

**模式参考**：`User.cs` — 继承 `Entity`，私有构造供 EF Core，业务方法修改状态。

### 1.2 新增 `WeChatOpenId` 值对象（可选轻量）

如果不需要复杂校验可以省略，直接用 `string`。建议保留作为值对象：

```
src/Hospital.Domain/ValueObjects/WeChatOpenId.cs
```

```csharp
namespace Hospital.Domain.ValueObjects;

public sealed record WeChatOpenId(string Value)
{
    public string Value { get; } = !string.IsNullOrWhiteSpace(Value)
        ? Value.Trim()
        : throw new ArgumentException("OpenId 不能为空", nameof(Value));
}
```

---

## 二、Application 层

### 2.1 新增 DTO

```
src/Hospital.Application/DTOs/WeChatAuthDTOs.cs
```

```csharp
namespace Hospital.Application.DTOs;

public sealed record WeChatLoginRequest(string Code);
public sealed record WeChatLoginResult(string TempToken, int ExpiresIn);

public sealed record BindPhoneRequest(string EncryptedData, string Iv, string TempToken);
public sealed record BindPhoneResult(
    string AccessToken,
    string RefreshToken,
    long PatientId,
    bool IsNew,
    List<PatientCandidate>? Candidates = null
);

public sealed record PatientCandidate(long PatientId, string Name, string IdCardNo);

public sealed record ConfirmPatientRequest(string TempToken, long PatientId);

public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record RefreshTokenResult(string AccessToken, string RefreshToken);
```

**模式参考**：`AuthenticationDTOs.cs` — `sealed record`。

### 2.2 新增应用服务接口

```
src/Hospital.Application/Services/WeChat/IWeChatAuthService.cs
```

```csharp
namespace Hospital.Application.Services.WeChat;

public interface IWeChatAuthService
{
    /// <summary>code → openid → 返回临时 token</summary>
    Task<WeChatLoginResult> LoginAsync(WeChatLoginRequest request);

    /// <summary>解密手机号 → 匹配/创建患者 → 签发 JWT</summary>
    Task<BindPhoneResult> BindPhoneAsync(BindPhoneRequest request);

    /// <summary>多患者匹配时确认选择</summary>
    Task<BindPhoneResult> ConfirmPatientAsync(ConfirmPatientRequest request);

    /// <summary>refresh_token → 新 access_token</summary>
    Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>撤销 refresh_token</summary>
    Task LogoutAsync(long patientId, string refreshToken);
}
```

**模式参考**：`IAuthenticationApplicationService.cs` — 接口定义在 `Services` 下。

---

## 三、Infrastructure 层

### 3.1 EF Core 配置

#### `WeChatAccountConfiguration.cs`

```csharp
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
```

#### `PatientRefreshTokenConfiguration.cs`（新增 entity 或直接用 owned entity）

建议直接在 DbContext 中用 `OnModelCreating` 配置，或单独建一个类。因不需要 Domain 实体，可以跳过 Entity 层，直接在 Infrastructure 中定义。

**简化方案**：不在 Domain 建实体，用 DbContext 的 `OnModelCreating` 直接建表。

### 3.2 WeChatHttpClient

```
src/Hospital.Infrastructure/ExternalServices/WeChatHttpClient.cs
```

```csharp
namespace Hospital.Infrastructure.ExternalServices;

public sealed class WeChatHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly string _appId;
    private readonly string _appSecret;

    public WeChatHttpClient(IConfiguration configuration, HttpClient httpClient)
    {
        _appId = configuration["WeChat:MiniProgram:AppId"]!;
        _appSecret = configuration["WeChat:MiniProgram:AppSecret"]!;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.weixin.qq.com/");
    }

    /// <summary>code 换 session</summary>
    public async Task<WeChatSessionResult> Code2SessionAsync(string code)
    {
        var url = $"/sns/jscode2session?appid={_appId}&secret={_appSecret}&js_code={code}&grant_type=authorization_code";
        var response = await _httpClient.GetFromJsonAsync<WeChatSessionResult>(url);
        if (response?.Errcode != 0 && response?.Errcode != null)
            throw new WeChatApiException(response.Errmsg ?? "code2session 失败");
        return response!;
    }

    /// <summary>解密手机号</summary>
    public string DecryptPhoneNumber(string encryptedData, string iv, string sessionKey)
    {
        // AES-128-CBC 解密（微信规范）
        // 返回 JSON: {"phoneNumber": "13800138000", ...}
    }
}

public sealed record WeChatSessionResult(
    string? Openid,
    string? SessionKey,
    string? Unionid,
    int? Errcode,
    string? Errmsg
);
```

### 3.3 PatientJwtService

```
src/Hospital.Infrastructure/ExternalServices/PatientJwtService.cs
```

```csharp
namespace Hospital.Infrastructure.ExternalServices;

public sealed class PatientJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly int _accessTokenExpiryMinutes;
    private readonly int _refreshTokenExpiryDays;

    public PatientJwtService(IConfiguration configuration) { /* 读取 WeChat:Jwt 配置 */ }

    public string GenerateAccessToken(long patientId, string openId)
    {
        // 与 JwtTokenService 相同模式，但：
        // - 独立 SecretKey
        // - Claim: sub = "patient:{patientId}", openid, userType = "patient"
        // - 有效期 = AccessTokenExpiryMinutes
    }

    public string GenerateRefreshToken()
    {
        // 随机 64 字节 hex
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        // 使用独立密钥验证
    }
}
```

**模式参考**：`JwtTokenService.cs`。

### 3.4 PatientNoService

```
src/Hospital.Infrastructure/ExternalServices/PatientNoService.cs
```

```
编号格式：P + YYYYMMDD + 4位序列
示例：P202605250001

规则：
- 每日重置，从 0001 开始
- 通过数据库查询当天最大号 + 1
- 线程安全：用数据库锁或乐观并发
```

**为什么选这个格式**：与种子数据 `P20250001` 风格一致，只是加日期更精确。

### 3.5 WeChatAuthService（核心实现）

```
src/Hospital.Infrastructure/ExternalServices/WeChatAuthService.cs
```

```csharp
public sealed class WeChatAuthService : IWeChatAuthService
{
    // 依赖：WeChatHttpClient, PatientJwtService, PatientNoService,
    //       HospitalDbContext, ILogger<WeChatAuthService>

    // LoginAsync: code → WeChatHttpClient.Code2SessionAsync → 生成临时 token（存 openid+session_key 到内存缓存）→ 返回 tempToken

    // BindPhoneAsync: 验证 tempToken → decrypt → 匹配 pat.Patients.Phone
    //   → 1个：绑定 WeChatAccount → 签发 JWT
    //   → 0个：先返回提示前端填姓名 → 再调 BindPhoneWithNameAsync
    //   → 多个：返回 candidates 列表

    // 内部方法 MatchPatientByPhone(string phone) → List<Patient>
    // 内部方法 CreatePatient(string name, string phone) → Patient（PatientNoService.NextNo） + isNew 标记
    // 内部方法 IssueToken(long patientId, string openId) → (accessToken, refreshToken)
    //   存 refreshToken 到 PatientRefreshTokens 表
}
```

**流程匹配**：严格对应需求文档第 4.1 节的三个分支。

### 3.6 DbContext 修改

```csharp
// HospitalDbContext.cs 新增：
public DbSet<WeChatAccount> WeChatAccounts => Set<WeChatAccount>();
```

`ApplyConfigurationsFromAssembly` 会自动发现 `WeChatAccountConfiguration.cs`。

---

## 四、API 层

### 4.1 MiniProgramAuthController

```
src/Hospital.Api/Controllers/MiniProgramAuthController.cs
```

```csharp
namespace Hospital.Api.Controllers;

[ApiController]
[Route("api/miniprogram/auth")]
public class MiniProgramAuthController : ControllerBase
{
    private readonly IWeChatAuthService _weChatAuthService;

    public MiniProgramAuthController(IWeChatAuthService weChatAuthService)
    {
        _weChatAuthService = weChatAuthService;
    }

    /// <summary>Step 1: code → temp token</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] WeChatLoginRequest request)
    {
        var result = await _weChatAuthService.LoginAsync(request);
        return Ok(result);
    }

    /// <summary>Step 2: 绑定手机号 → 匹配/创建 → JWT</summary>
    [HttpPost("bind-phone")]
    public async Task<IActionResult> BindPhone([FromBody] BindPhoneRequest request)
    {
        var result = await _weChatAuthService.BindPhoneAsync(request);
        return Ok(result);
    }

    /// <summary>Step 2b: 多患者时确认选择</summary>
    [HttpPost("confirm-patient")]
    public async Task<IActionResult> ConfirmPatient([FromBody] ConfirmPatientRequest request)
    {
        var result = await _weChatAuthService.ConfirmPatientAsync(request);
        return Ok(result);
    }

    /// <summary>刷新 token</summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _weChatAuthService.RefreshTokenAsync(request);
        return Ok(result);
    }

    /// <summary>注销</summary>
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = "PatientJwt")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var patientId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _weChatAuthService.LogoutAsync(patientId, request.RefreshToken);
        return Ok();
    }
}
```

**模式参考**：`AuthenticationController.cs` — 但新增独立 Controller 而非修改现有。

### 4.2 Program.cs 修改

在 `builder.Services` 区域追加：

```csharp
// 微信登录服务
builder.Services.AddHttpClient<WeChatHttpClient>();
builder.Services.AddSingleton<PatientJwtService>();
builder.Services.AddSingleton<PatientNoService>();
builder.Services.AddScoped<IWeChatAuthService, WeChatAuthService>();
```

同时需要注册患者 JWT 的 `AuthenticationScheme`：

```csharp
// 在现有 JWT 注册之后，追加患者 JWT 方案
builder.Services.AddAuthentication()
    .AddJwtBearer("PatientJwt", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("WeChat:Jwt");
        // ... 用独立密钥配置
    });
```

**注意**：患者 JWT 不走默认 `JwtBearerDefaults.AuthenticationScheme`，而是命名方案 `"PatientJwt"`，与后台 JWT 隔离。

### 4.3 appsettings.json 修改

```json
"WeChat": {
  "MiniProgram": {
    "AppId": "wx71380c520e3e0777",
    "AppSecret": ""
  },
  "Jwt": {
    "SecretKey": "【生成一个 32 位以上的随机密钥】",
    "Issuer": "HospitalMiniProgram",
    "AccessTokenExpiryMinutes": 120,
    "RefreshTokenExpiryDays": 30,
    "TempTokenExpiryMinutes": 5
  }
}
```

---

## 五、数据库

### 5.1 迁移脚本

```sql
-- database/010_wechat_login.sql

-- 微信账号关联表
CREATE TABLE [sec].[WeChatAccounts] (
    [Id]           bigint NOT NULL IDENTITY,
    [OpenId]       nvarchar(128) NOT NULL,
    [UnionId]      nvarchar(128) NULL,
    [PatientId]    bigint NOT NULL,
    [NickName]     nvarchar(100) NULL,
    [AvatarUrl]    nvarchar(500) NULL,
    [Phone]        nvarchar(32) NULL,
    [CreatedAt]    datetime2 NOT NULL DEFAULT GETDATE(),
    [LastLoginAt]  datetime2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_WeChatAccounts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WeChatAccounts_Patients_PatientId]
        FOREIGN KEY ([PatientId]) REFERENCES [pat].[Patients]([Id])
);
CREATE UNIQUE INDEX [IX_WeChatAccounts_OpenId] ON [sec].[WeChatAccounts] ([OpenId]);

-- refresh_token 表
CREATE TABLE [sec].[PatientRefreshTokens] (
    [Id]           bigint NOT NULL IDENTITY,
    [PatientId]    bigint NOT NULL,
    [Token]        nvarchar(500) NOT NULL,
    [ExpiresAt]    datetime2 NOT NULL,
    [CreatedAt]    datetime2 NOT NULL DEFAULT GETDATE(),
    [RevokedAt]    datetime2 NULL,
    CONSTRAINT [PK_PatientRefreshTokens] PRIMARY KEY ([Id])
);
CREATE INDEX [IX_PatientRefreshTokens_PatientId] ON [sec].[PatientRefreshTokens] ([PatientId]);
```

---

## 六、小程序前端

### 6.1 新增 wechat-auth-service.js

```
hospital-miniapp/services/wechat-auth-service.js
```

```javascript
const { api } = require('../utils/api')

const WeChatAuthService = {
  // Step 1: code → temp token
  login(code) {
    return api.post('/api/miniprogram/auth/login', { code })
  },

  // Step 2: 绑定手机号
  bindPhone(encryptedData, iv, tempToken) {
    return api.post('/api/miniprogram/auth/bind-phone', { encryptedData, iv, tempToken })
  },

  // Step 2b: 多患者确认
  confirmPatient(tempToken, patientId) {
    return api.post('/api/miniprogram/auth/confirm-patient', { tempToken, patientId })
  },

  // 刷新 token
  refresh(refreshToken) {
    return api.post('/api/miniprogram/auth/refresh', { refreshToken })
  },

  // 注销
  logout(refreshToken) {
    return api.post('/api/miniprogram/auth/logout', { refreshToken })
  }
}

module.exports = WeChatAuthService
```

### 6.2 login.js 改造

**去掉账号密码逻辑，只保留微信登录**：

```
微信按钮点击:
  1. wx.login() → 获取 code
  2. 调 WeChatAuthService.login(code) → 得到 tempToken
  3. wx.getPhoneNumber() → encryptedData + iv
  4. 调 WeChatAuthService.bindPhone(encryptedData, iv, tempToken)
     → 返回结果处理:
       candidates = null, isNew = false → 直接进首页
       candidates 有值 → 展示选择列表 → 选后调 confirmPatient
       isNew = true → 弹姓名输入框 → 重新调 bindPhone 提交姓名 → 进首页 + 提示完善信息
  5. 失败 → 提示错误
```

### 6.3 api.js 改造

```javascript
// 在 401 处理逻辑中增加 refresh_token 自动刷新

// 关键改动：
// 1. 401 时，如果有 refresh_token，自动调 /api/miniprogram/auth/refresh
// 2. 刷新成功 → 更新 token → 重放原请求
// 3. 刷新失败 → 清除所有 Storage → 跳登录页
```

小程序只有患者使用，无需区分 userType，所有请求统一走患者 JWT + refresh_token 刷新策略。

### 6.4 app.js 改造

```javascript
// 增加：
globalData: {
  userInfo: null,
  token: '',
  refreshToken: '',    // 新增
  patients: [],
  currentPatient: null
}

// onLaunch 增加读取 refreshToken
// setUserInfo 增加保存 refreshToken
// logout 增加清除 refreshToken（含 patients）
```

小程序只有患者使用，无需 `userType` 字段。

---

## 七、实施顺序

建议按此顺序开发，每步可独立验证：

```
Step 1 ─── 数据库脚本 → 执行建表
Step 2 ─── Domain: WeChatAccount 实体
Step 3 ─── Application: DTO + IWeChatAuthService 接口
Step 4 ─── Infrastructure:
             4a. WeChatAccountConfiguration + DbContext 修改
             4b. PatientJwtService
             4c. PatientNoService
             4d. WeChatHttpClient
             4e. WeChatAuthService（核心）
Step 5 ─── API: MiniProgramAuthController + Program.cs 注册 + appsettings
Step 6 ─── 小程序: login.js/wxml 重构 + api.js 拦截器 + app.js
          （可先用模拟数据调通，联调时对接真实后端）
Step 7 ─── 联调 + 真机测试
```
