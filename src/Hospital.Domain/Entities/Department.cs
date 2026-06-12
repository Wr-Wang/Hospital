using Hospital.Domain.Enums;
using Hospital.Domain.ValueObjects;

namespace Hospital.Domain.Entities;

public class Department : Entity
{
    /// <summary>科室编码，同一院区内唯一</summary>
    public DepartmentCode Code { get; private set; }
    /// <summary>科室名称</summary>
    public string Name { get; private set; }
    /// <summary>上级科室 ID（null 表示根节点）</summary>
    public long? ParentId { get; private set; }
    /// <summary>所属院区 ID</summary>
    public long CampusId { get; private set; }
    /// <summary>科室类型（门诊/住院/医技/行政/药房）</summary>
    public DepartmentType Type { get; private set; }
    /// <summary>启用状态，停用时级联校验</summary>
    public bool IsActive { get; private set; } = true;

    // 导航属性（EF Core 延迟加载用）
    public Department? Parent { get; private set; }
    public List<Department> Children { get; private set; } = new();

    // EF Core 无参构造
    private Department()
    {
        Code = default!;
        Name = default!;
    }

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
