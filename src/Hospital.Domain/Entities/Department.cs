using Hospital.Domain.Enums;
using Hospital.Domain.ValueObjects;

namespace Hospital.Domain.Entities;

public class Department : Entity
{
    public DepartmentCode Code { get; private set; }
    public string Name { get; private set; }
    public long? ParentId { get; private set; }
    public long CampusId { get; private set; }
    public DepartmentType Type { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation property
    public Department? Parent { get; private set; }
    public List<Department> Children { get; private set; } = new();

    private Department()
    {
        Code = default!;
        Name = default!;
    } // For EF Core

    public Department(DepartmentCode code, string name, long campusId, DepartmentType type, long? parentId = null)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CampusId = campusId;
        Type = type;
        ParentId = parentId;
    }

    public void UpdateInfo(string name, DepartmentType type, long? parentId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        ParentId = parentId;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
