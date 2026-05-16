namespace Hospital.Domain.Entities;

public class DictionaryItem : Entity
{
    public long TypeId { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public long? ParentId { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation
    public DictionaryType? Type { get; private set; }

    private DictionaryItem()
    {
        Code = default!;
        Name = default!;
    } // For EF Core

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
