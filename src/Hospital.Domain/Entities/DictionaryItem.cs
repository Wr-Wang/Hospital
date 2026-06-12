namespace Hospital.Domain.Entities;

public class DictionaryItem : Entity
{
    /// <summary>所属字典类型 ID</summary>
    public long TypeId { get; private set; }
    /// <summary>字典项编码，同类型内唯一</summary>
    public string Code { get; private set; }
    /// <summary>字典项名称（展示值）</summary>
    public string Name { get; private set; }
    /// <summary>上级字典项 ID（支持树形字典）</summary>
    public long? ParentId { get; private set; }
    /// <summary>排序号</summary>
    public int SortOrder { get; private set; }
    /// <summary>启用状态</summary>
    public bool IsActive { get; private set; } = true;

    // 导航属性
    public DictionaryType? Type { get; private set; }

    // EF Core 无参构造
    private DictionaryItem()
    {
        Code = default!;
        Name = default!;
    }

    public DictionaryItem(long typeId, string code, string name, long? parentId, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("字典项编码不能为空", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("字典项名称不能为空", nameof(name));

        TypeId = typeId;
        Code = code;
        Name = name;
        ParentId = parentId;
        SortOrder = sortOrder;
    }

    public void UpdateInfo(string name, long? parentId, int sortOrder)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ParentId = parentId;
        SortOrder = sortOrder;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
