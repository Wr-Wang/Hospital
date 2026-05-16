namespace Hospital.Domain.Entities;

public class DictionaryType : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private DictionaryType()
    {
        Code = default!;
        Name = default!;
    } // For EF Core

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
