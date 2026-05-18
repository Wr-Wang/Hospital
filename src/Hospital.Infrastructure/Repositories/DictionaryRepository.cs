using Hospital.Application.Repositories;
using Hospital.Domain;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class DictionaryRepository : IDictionaryRepository
{
    private readonly List<DictionaryType> _types = new();
    private readonly List<DictionaryItem> _items = new();

    public DictionaryRepository()
    {
        var typeSeeds = new DictionaryType[]
        {
            new("GENDER", "性别", "患者性别"),
            new("LICENSE_TYPE", "执业资质类型", "医护人员执业资质分类"),
            new("MARITAL_STATUS", "婚姻状况", "患者婚姻状况"),
            new("ALLERGEN", "过敏原", "药物/物质过敏原"),
            new("DRUG_UNIT", "药品单位", "药品计量单位"),
        };
        for (int i = 0; i < typeSeeds.Length; i++)
        {
            typeof(Entity).GetProperty("Id")?.SetValue(typeSeeds[i], i + 1);
            _types.Add(typeSeeds[i]);
        }

        var itemSeeds = new DictionaryItem[]
        {
            // GENDER (typeId=1)
            new(1, "M", "男", null, 1),
            new(1, "F", "女", null, 2),
            // LICENSE_TYPE (typeId=2)
            new(2, "DOC", "执业医师", null, 1),
            new(2, "NURSE", "执业护士", null, 2),
            new(2, "PHARM", "药师", null, 3),
            new(2, "TECH", "医技", null, 4),
            // MARITAL_STATUS (typeId=3)
            new(3, "MARRIED", "已婚", null, 1),
            new(3, "UNMARRIED", "未婚", null, 2),
            new(3, "DIVORCED", "离异", null, 3),
            new(3, "WIDOWED", "丧偶", null, 4),
            // ALLERGEN (typeId=4)
            new(4, "PCN", "青霉素", null, 1),
            new(4, "SFA", "磺胺类药物", null, 2),
            new(4, "CEPH", "头孢菌素", null, 3),
            new(4, "IODINE", "碘造影剂", null, 4),
            // DRUG_UNIT (typeId=5)
            new(5, "TAB", "片", null, 1),
            new(5, "CAP", "粒", null, 2),
            new(5, "ML", "毫升", null, 3),
            new(5, "G", "克", null, 4),
        };
        for (int i = 0; i < itemSeeds.Length; i++)
        {
            typeof(Entity).GetProperty("Id")?.SetValue(itemSeeds[i], i + 1);
            _items.Add(itemSeeds[i]);
        }
    }

    // ===== 字典类型 =====

    public Task<DictionaryType?> GetTypeByIdAsync(long id)
    {
        return Task.FromResult(_types.FirstOrDefault(t => t.Id == id));
    }

    public Task<DictionaryType?> GetTypeByCodeAsync(string code)
    {
        return Task.FromResult(_types.FirstOrDefault(t => t.Code == code));
    }

    public Task<List<DictionaryType>> GetAllTypesAsync()
    {
        return Task.FromResult(_types.ToList());
    }

    public Task AddTypeAsync(DictionaryType type)
    {
        type.GetType().GetProperty("Id")?.SetValue(type, _types.Count + 1);
        _types.Add(type);
        return Task.CompletedTask;
    }

    public Task UpdateTypeAsync(DictionaryType type)
    {
        var index = _types.FindIndex(t => t.Id == type.Id);
        if (index >= 0)
            _types[index] = type;
        return Task.CompletedTask;
    }

    public Task DeleteTypeAsync(long id)
    {
        var type = _types.FirstOrDefault(t => t.Id == id);
        if (type is not null)
            _types.Remove(type);
        return Task.CompletedTask;
    }

    // ===== 字典项 =====

    public Task<DictionaryItem?> GetItemByIdAsync(long id)
    {
        return Task.FromResult(_items.FirstOrDefault(i => i.Id == id));
    }

    public Task<List<DictionaryItem>> GetItemsByTypeIdAsync(long typeId)
    {
        var result = _items.Where(i => i.TypeId == typeId).OrderBy(i => i.SortOrder).ToList();
        return Task.FromResult(result);
    }

    public Task<List<DictionaryItem>> GetItemsByTypeCodeAsync(string typeCode)
    {
        var type = _types.FirstOrDefault(t => t.Code == typeCode);
        if (type is null)
            return Task.FromResult(new List<DictionaryItem>());

        var result = _items.Where(i => i.TypeId == type.Id).OrderBy(i => i.SortOrder).ToList();
        return Task.FromResult(result);
    }

    public Task AddItemAsync(DictionaryItem item)
    {
        item.GetType().GetProperty("Id")?.SetValue(item, _items.Count + 1);
        _items.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateItemAsync(DictionaryItem item)
    {
        var index = _items.FindIndex(i => i.Id == item.Id);
        if (index >= 0)
            _items[index] = item;
        return Task.CompletedTask;
    }

    public Task DeleteItemAsync(long id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item is not null)
            _items.Remove(item);
        return Task.CompletedTask;
    }
}
