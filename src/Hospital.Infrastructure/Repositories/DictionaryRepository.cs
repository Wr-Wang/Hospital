using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class DictionaryRepository : IDictionaryRepository
{
    private readonly List<DictionaryType> _types = new();
    private readonly List<DictionaryItem> _items = new();

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
