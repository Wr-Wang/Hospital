namespace Hospital.Infrastructure.ExternalServices;

/// <summary>
/// 内存登录账号存储（演示用）。
/// 注意：StaffId / CampusId 必须与数据库种子（mdm.Staff / mdm.Campuses）保持一致，
/// 登录返回的 Id 即 StaffId，App 端 CurrentUserId 的语义就是"当前操作人员 Id"。
/// </summary>
public sealed class LocalUserStore
{
    private readonly List<LocalUser> _users = new()
    {
        new LocalUser
        {
            Id = 1, StaffId = 1, CampusId = 1,
            LoginName = "admin",
            Password = "admin123",
            DisplayName = "系统管理员",
            CampusName = "总院区",
            Roles = new[] { "ADMIN" },
            Permissions = new[] { "sys.shell.use", "sys.security.manage",
                "mdm.campus.manage", "mdm.dept.manage", "mdm.staff.manage", "mdm.dict.manage",
                "pat.register", "pat.search",
                "opd.schedule", "opd.register", "opd.encounter",
                "pha.dispense", "fin.cash" }
        },
        new LocalUser
        {
            Id = 2, StaffId = 2, CampusId = 1,
            LoginName = "doctor",
            Password = "doctor123",
            DisplayName = "张医生",
            CampusName = "总院区",
            Roles = new[] { "DOCTOR" },
            Permissions = new[] { "sys.shell.use", "pat.search", "opd.encounter", "opd.schedule" }
        },
        new LocalUser
        {
            Id = 3, StaffId = 3, CampusId = 1,
            LoginName = "reg",
            Password = "reg123",
            DisplayName = "李挂号",
            CampusName = "总院区",
            Roles = new[] { "REGISTRATION" },
            Permissions = new[] { "sys.shell.use", "pat.register", "pat.search", "opd.schedule", "opd.register" }
        },
        new LocalUser
        {
            Id = 4, StaffId = 4, CampusId = 1,
            LoginName = "cash",
            Password = "cash123",
            DisplayName = "赵收费",
            CampusName = "总院区",
            Roles = new[] { "CASHIER" },
            Permissions = new[] { "sys.shell.use", "fin.cash" }
        },
        new LocalUser
        {
            Id = 5, StaffId = 5, CampusId = 1,
            LoginName = "pharm",
            Password = "pharm123",
            DisplayName = "王药房",
            CampusName = "总院区",
            Roles = new[] { "PHARMACY" },
            Permissions = new[] { "sys.shell.use", "pha.dispense" }
        },
    };

    public LocalUser? FindByLoginName(string loginName)
    {
        return _users.FirstOrDefault(u =>
            string.Equals(u.LoginName, loginName, StringComparison.OrdinalIgnoreCase));
    }
}

public sealed class LocalUser
{
    /// <summary>登录账号 Id（与 StaffId 保持一致）</summary>
    public long Id { get; init; }
    /// <summary>对应 mdm.Staff 的人员 Id，登录返回即此值</summary>
    public long StaffId { get; init; }
    /// <summary>对应 mdm.Campuses 的院区 Id</summary>
    public long CampusId { get; init; }
    public string LoginName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string CampusName { get; init; } = string.Empty;
    public string[] Roles { get; init; } = Array.Empty<string>();
    public string[] Permissions { get; init; } = Array.Empty<string>();
}
