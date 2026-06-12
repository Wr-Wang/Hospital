namespace Hospital.Infrastructure.ExternalServices;

public sealed class LocalUserStore
{
    private readonly List<LocalUser> _users = new()
    {
        new LocalUser
        {
            Id = 1,
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
            Id = 2,
            LoginName = "doctor",
            Password = "doctor123",
            DisplayName = "张医生",
            CampusName = "总院区",
            Roles = new[] { "DOCTOR" },
            Permissions = new[] { "sys.shell.use", "pat.search", "opd.encounter", "opd.schedule" }
        },
        new LocalUser
        {
            Id = 3,
            LoginName = "reg",
            Password = "reg123",
            DisplayName = "李挂号",
            CampusName = "总院区",
            Roles = new[] { "REGISTRATION" },
            Permissions = new[] { "sys.shell.use", "pat.register", "pat.search", "opd.schedule", "opd.register" }
        },
        new LocalUser
        {
            Id = 4,
            LoginName = "pharm",
            Password = "pharm123",
            DisplayName = "王药房",
            CampusName = "总院区",
            Roles = new[] { "PHARMACY" },
            Permissions = new[] { "sys.shell.use", "pha.dispense" }
        },
        new LocalUser
        {
            Id = 5,
            LoginName = "cash",
            Password = "cash123",
            DisplayName = "赵收费",
            CampusName = "总院区",
            Roles = new[] { "CASHIER" },
            Permissions = new[] { "sys.shell.use", "fin.cash" }
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
    public long Id { get; init; }
    public string LoginName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string CampusName { get; init; } = string.Empty;
    public string[] Roles { get; init; } = Array.Empty<string>();
    public string[] Permissions { get; init; } = Array.Empty<string>();
}
