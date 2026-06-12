# 微信登录功能 — 需求文档

> 基于现有医院 HIS 系统 + `hospital-miniapp` 微信小程序，增加患者微信登录能力。

---

## 1. 当前状态

| 项目 | 现状 |
|------|------|
| 小程序认证 | 仅账号密码登录（`POST /api/authentication/login`），调 `LocalAuthenticationService` |
| 后端认证 | JWT Token（24h 过期），**无 refresh_token** |
| 后台用户 | `sec.Users` — 医生/护士/收费员等，通过 WPF 桌面端 + 账号密码登录 |
| 患者数据 | `pat.Patients` — 含手机号、身份证号 |
| 微信绑定 | 无 — 没有任何 openid 关联表 |
| 小程序 AppID | `wx71380c520e3e0777`（已注册） |

---

## 2. 核心结论

| 决策 | 内容 |
|------|------|
| 微信登录范围 | **仅限患者**。后台工作人员继续使用 WPF 桌面端 + 账号密码 |
| 登录方式 | 仅微信登录，小程序不再提供账号密码入口 |
| 绑定策略 | 微信手机号自动匹配院内患者档案，匹配不到则用手机号+姓名自动创建 |
| 多患者匹配 | 返回患者列表，由患者选择确认 |
| 数据归属 | 所有业务数据挂在院内患者 ID 下，openid 仅做认证凭据 |
| 会话保持 | 新增 refresh_token 机制，实现静默续期 |
| 患者与工作人员隔离 | 两套用户体系完全独立：患者走 `pat.Patients` + 微信，工作人员走 `sec.Users` + 账号密码 |
| JWT 密钥 | 患者 JWT 使用独立密钥，与现有后台 JWT 隔离 |
| refresh_token | 30 天有效期 |
| 就诊人上限 | 沿用现有 5 人上限 |
| AppSecret | 已获取 |

---

## 3. 登录页设计

只保留微信一键登录，去掉账号密码入口：

```
┌─────────────────────────┐
│      XX医院              │
│  预约挂号服务平台         │
│                         │
│  ┌─────────────────────┐ │
│  │  微信一键登录        │ │  ← wx.login() + 手机号授权
│  └─────────────────────┘ │
│                         │
│  暂不登录，先浏览         │
└─────────────────────────┘
```

---

## 4. 微信登录流程

### 4.1 首次使用

```
患者点击"微信一键登录"
    │
    ├── wx.login() → 获取 code
    │
    ├── wx.getPhoneNumber() → 加密数据（encryptedData + iv）
    │   （需用户点击"允许"）
    │
    ├── POST /api/miniprogram/auth/login { code }
    │   后端：调微信 code2session → 获得 openid + session_key
    │   返回：{ tempToken（5分钟有效） }
    │
    ├── POST /api/miniprogram/auth/bind-phone
    │   参数：{ encryptedData, iv, tempToken }
    │   后端：解密手机号 → 匹配 pat.Patients.Phone
    │   │
    │   ├── 匹配到 1 个患者
    │   │   → 绑定 openid ↔ patientId
    │   │   → 签发患者 JWT
    │   │   → 返回 { userType:"patient", patientId, isNew:false }
    │   │
    │   ├── 匹配到多个患者
    │   │   → 返回患者列表 [{ patientId, name, idCardNo }]
    │   │   → 小程序展示列表让患者选择
    │   │   → 患者选择后绑定 openid ↔ patientId → 签发 JWT
    │   │   → 返回 { userType:"patient", patientId, isNew:false }
    │   │
    │   └── 匹配到 0 个患者
    │       → 小程序弹出表单让患者填写姓名
    │       → 前端提交 { name, tempToken } 到 /auth/bind-phone
    │       → 用手机号+姓名创建"待完善"患者
    │       → 绑定 openid ↔ patientId
    │       → 签发患者 JWT
    │       → 返回 { userType:"patient", patientId, isNew:true }
    │
    └── 小程序收到 JWT
        → 存入 Storage
        → 进入首页
        → isNew = true → 提示"请完善就诊人信息"
```

### 4.2 再次使用（静默登录）

```
打开小程序
    │
    ├── Storage 中有有效 token → 直接进入首页
    │
    ├── token 过期（API 返回 401）
    │   ├── 有 refresh_token → 静默刷新 → 重放请求
    │   └── refresh_token 过期 → 清除 token → 显示登录页
    │
    └── 无 token → 显示登录页
```

### 4.3 核心规则

| 规则 | 说明 |
|------|------|
| 一个 openid 只能绑定一个 patientId | 微信与患者 1:1 绑定 |
| 一个 patientId 可被多个 openid 绑定 | 如家属代操作 |
| 绑定后再次使用 | 走静默流程，不再弹窗授权 |
| 手机号匹配到 0 个患者 | 弹姓名输入框 → 用手机号+姓名创建"待完善"患者 |
| 手机号匹配到多个患者 | 返回列表让患者选择 |
| openid 不做业务主键 | 业务数据统一用 patientId |

---

## 5. Token 策略

新增 refresh_token，与微信登录配合：

| Token | 有效期 | 存放位置 | 用途 |
|-------|--------|---------|------|
| access_token | 2 小时 | 内存 + Storage | API 请求认证头 |
| refresh_token | 30 天 | Storage | 静默刷新 access_token |
| 临时 token | 5 分钟 | 内存 | 手机号绑定环节临时凭证 |

患者 JWT 载荷：

```json
{
  "sub": "patient:{patientId}",
  "openid": "微信 openid",
  "userType": "patient",
  "iat": ...,
  "exp": ...
}
```

**密钥独立**：患者 JWT 与现有后台 JWT 使用不同的 SecretKey，避免两类 token 互串。

---

## 6. 后端改动清单

### 6.1 数据库

```sql
-- 微信账号关联表（患者专用）
CREATE TABLE [sec].[WeChatAccounts] (
    [Id]           bigint NOT NULL IDENTITY,
    [OpenId]       nvarchar(128) NOT NULL,
    [UnionId]      nvarchar(128) NULL,           -- 预留
    [PatientId]    bigint NOT NULL,               -- FK → pat.Patients.Id
    [NickName]     nvarchar(100) NULL,
    [AvatarUrl]    nvarchar(500) NULL,
    [Phone]        nvarchar(32) NULL,
    [CreatedAt]    datetime2 NOT NULL DEFAULT GETDATE(),
    [LastLoginAt]  datetime2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_WeChatAccounts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WeChatAccounts_Patients] FOREIGN KEY ([PatientId]) REFERENCES [pat].[Patients]([Id])
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
```

### 6.2 Application 层

| 接口 | 方法 | 说明 |
|------|------|------|
| `IWeChatAuthService` | `LoginAsync(LoginRequest)` | code → openid → tempToken |
| | `BindPhoneAsync(BindPhoneRequest)` | 解密手机号 → 匹配/创建患者 → JWT |
| | `ConfirmPatientAsync(ConfirmPatientRequest)` | 多患者匹配时，患者选择后确认绑定 |
| | `RefreshTokenAsync(RefreshTokenRequest)` | refresh_token 换新 access_token |
| | `LogoutAsync(LogoutRequest)` | 撤销 refresh_token |

| DTO | 说明 |
|-----|------|
| `WeChatLoginRequest` | `{ code }` |
| `WeChatLoginResult` | `{ tempToken, expiresIn }` |
| `BindPhoneRequest` | `{ encryptedData, iv, tempToken }` — 自动匹配或匹配到 0 个时调用 |
| `BindPhoneResult` | `{ accessToken, refreshToken, patientId, isNew, candidates }` |
| `ConfirmPatientRequest` | `{ tempToken, patientId }` — 多患者匹配时确认 |
| `RefreshTokenRequest` | `{ refreshToken }` |
| `RefreshTokenResult` | `{ accessToken, refreshToken }` |

### 6.3 Infrastructure 层

| 类 | 说明 |
|----|------|
| `WeChatHttpClient` | 封装微信 `code2session`、数据解密 |
| `WeChatAuthService` | 实现 `IWeChatAuthService` |
| `PatientJwtService` | 生成/验证患者 JWT（密钥与后台 JWT 独立） |
| `EfWeChatAccountRepository` | 微信账号仓储 |
| `EfPatientRefreshTokenRepository` | refresh_token 仓储 |

### 6.4 API 层

| 端点 | 方法 | 说明 | 认证 |
|------|------|------|------|
| `POST /api/miniprogram/auth/login` | 匿名 | code → temp token | 无 |
| `POST /api/miniprogram/auth/bind-phone` | 临时 token | 绑手机号 → 匹配 → 签发/候选列表 | 临时 token |
| `POST /api/miniprogram/auth/confirm-patient` | 临时 token | 多患者时确认选择 → 签发 JWT | 临时 token |
| `POST /api/miniprogram/auth/refresh` | 匿名 | refresh → 新 access_token | 无 |
| `POST /api/miniprogram/auth/logout` | 患者 JWT | 撤销 refresh_token | 患者 JWT |

新增 `MiniProgramAuthController`，独立于现有 `AuthenticationController`。

### 6.5 配置

```json
"WeChat": {
  "MiniProgram": {
    "AppId": "wx71380c520e3e0777",
    "AppSecret": "【待填入】"
  },
  "Jwt": {
    "SecretKey": "【与后台 JWT 独立的密钥】",
    "Issuer": "HospitalMiniProgram",
    "AccessTokenExpiryMinutes": 120,
    "RefreshTokenExpiryDays": 30,
    "TempTokenExpiryMinutes": 5
  }
}
```

---

## 7. 小程序前端改动清单

### 7.1 登录页 (`pages/login/login`)

- 重新设计：去掉账号密码表单，仅保留微信一键登录按钮
- 微信登录全流程：`wx.login()` → `wx.getPhoneNumber()` → 提交后端
- 匹配到 0 个患者时：弹姓名输入框，提交姓名后走创建流程
- 匹配到多个患者时：展示候选列表（姓名 + 脱敏身份证号）让患者选择
- 新用户引导：`isNew = true` 时跳转完善就诊人信息页

### 7.2 API 请求封装 (`utils/api.js`)

- 401 自动拦截 + refresh_token 静默刷新
- 刷新失败 → 清除 Storage → 跳登录页

### 7.3 全局状态 (`app.js`)

- 增加 refresh_token 存取
- 更新 `setUserInfo` / `logout` 逻辑

---

## 8. 患者与工作人员资料隔离

系统存在两套完全独立的用户体系：

| 维度 | 患者（微信登录） | 工作人员（账号密码） |
|------|-----------------|-------------------|
| 认证方式 | 微信一键登录 | 用户名 + 密码 |
| 身份标识 | `patientId` → `pat.Patients` | `userId` → `sec.Users` |
| JWT 类型 | 患者 JWT（独立密钥） | 后台 JWT（独立密钥） |
| 使用的客户端 | 微信小程序 | WPF 桌面端 + Web 管理端 |
| 业务范围 | 预约挂号、排队、查报告、缴费 | 接诊、开药、收费、排班、管理 |
| 个人资料管理 | 小程序「个人中心」→ 就诊人管理 | WPF 系统管理模块 |

**两套体系互不干扰**：
- 患者不能通过微信登录获得后台权限
- 数据库表完全分离：`pat.Patients` / `sec.WeChatAccounts` / `sec.PatientRefreshTokens` 属于患者；`sec.Users` / `sec.Roles` 属于工作人员

---

## 9. 与现有系统关系

| 现有组件 | 关系 |
|---------|------|
| `AuthenticationController` | 不变，继续服务后台人员账号密码登录 |
| `LocalAuthenticationService` | 不变，后台用户认证，与患者体系无关 |
| `JwtTokenService` | 不变，继续生成后台 JWT。患者 JWT 由新增 `PatientJwtService` 独立生成（不同密钥） |
| `[Authorize]` 标注的 Controller | 不受影响，默认认证方案只验证后台 JWT，患者 JWT 无法通过 |
| WPF 客户端 / Web 管理端 | 不受任何影响，仍使用账号密码登录 |

**两套 JWT 互不互通**：患者 JWT 使用独立的 `"PatientJwt"` 认证方案，与默认的 JWT Bearer 方案完全隔离。

---

## 10. 实施阶段

```
Phase 1 ───── 后端（3-4 天）
  • 数据库：WeChatAccounts + PatientRefreshTokens 表
  • WeChatHttpClient（code2session + 解密）
  • WeChatAuthService（login / bind-phone / confirm-patient / refresh / logout）
  • PatientJwtService（独立密钥）
  • MiniProgramAuthController
  • 单元测试

Phase 2 ───── 小程序前端（2-3 天）
  • 登录页重构（仅微信登录）
  • 多患者选择界面
  • 新患者填写姓名界面
  • API 拦截器增加 401 自动刷新
  • 新用户引导流程
  • 真机调试

Phase 3 ───── 联调 + 审核（3-5 天）
  • 部署测试环境（HTTPS）
  • 微信后台配置 request 合法域名
  • 回归测试
  • 提交小程序审核
```

---

## 11. 本期不做的事

| 事项 | 原因 |
|------|------|
| 微信支付 | 需商户号资质，后续独立规划 |
| 微信订阅消息 | 需模板审核，后续规划 |
| 后台人员微信登录 | 本期专注患者端 |
| 在线建档（完整版） | 本次仅"匹配不到时自动创建基本档案" |

---

## 12. 待确认事项

- [x] **手机号匹配到多个患者** → 返回列表让患者选择 ✓
- [x] **自动创建患者字段** → 手机号 + 姓名 ✓
- [x] **患者 JWT 密钥独立** → 同意 ✓
- [x] **AppSecret** → 已获取 ✓
- [x] **refresh_token 有效期** → 30天 ✓
- [x] **就诊人上限** → 沿用 5 人上限 ✓
- [ ] **匹配到多个患者时的列表展示字段**：列表中展示哪些信息？(patientId + name + idCardNo 是否足够？是否要脱敏？)
- [ ] **新创建患者的默认值**：`PatientNo` 生成规则？性别/出生日期从微信获取还是留空让患者后续完善？
