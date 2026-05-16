namespace Hospital.Domain.Entities;

/// <summary>角色实体，包含权限集合</summary>
public class Role : Entity
{
    private readonly List<string> _permissions = new();

    // EF Core
    private Role() { }

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public IReadOnlyList<string> Permissions => _permissions.AsReadOnly();

    public void SetPermissions(IEnumerable<string> permissions)
    {
        _permissions.Clear();
        _permissions.AddRange(permissions);
    }
}
