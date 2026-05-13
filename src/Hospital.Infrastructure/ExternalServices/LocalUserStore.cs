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
            Permissions = new[] { "sys.shell.use", "mdm.campus.manage", "opd.register.work" }
        },
        new LocalUser
        {
            Id = 2,
            LoginName = "doctor",
            Password = "doctor123",
            DisplayName = "张医生",
            CampusName = "总院区",
            Roles = new[] { "DOCTOR" },
            Permissions = new[] { "sys.shell.use", "opd.register.work" }
        }
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
