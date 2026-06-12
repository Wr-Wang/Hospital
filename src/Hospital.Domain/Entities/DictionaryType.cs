namespace Hospital.Domain.Entities;

public class DictionaryType : Entity
{
    /// <summary>字典类型编码（如 "ICD10"、"DRUG_UNIT"），系统内唯一</summary>
    public string Code { get; private set; }
    /// <summary>字典类型名称</summary>
    public string Name { get; private set; }
    /// <summary>描述说明</summary>
    public string? Description { get; private set; }
    /// <summary>启用状态，停用后其下字典项不可用</summary>
    public bool IsActive { get; private set; } = true;

    // EF Core 无参构造
    private DictionaryType()
    {
        Code = default!;
        Name = default!;
    }

    public DictionaryType(string code, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("字典类型编码不能为空", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("字典类型名称不能为空", nameof(name));

        Code = code;
        Name = name;
        Description = description;
    }

    public void UpdateInfo(string name, string? description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
