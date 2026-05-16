namespace Hospital.Domain.Entities;

/// <summary>系统用户实体</summary>
public class User : Entity
{
    private readonly List<string> _roles = new();

    // EF Core
    private User() { }

    public User(string loginName, string password, string displayName, string campusName)
    {
        LoginName = loginName;
        Password = password;
        DisplayName = displayName;
        CampusName = campusName;
        IsActive = true;
    }

    public string LoginName { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string CampusName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public IReadOnlyList<string> Roles => _roles.AsReadOnly();

    public void SetRoles(IEnumerable<string> roles)
    {
        _roles.Clear();
        _roles.AddRange(roles);
    }

    public void ChangePassword(string newPassword)
    {
        Password = newPassword;
    }

    public void SetActive(bool active)
    {
        IsActive = active;
    }
}
