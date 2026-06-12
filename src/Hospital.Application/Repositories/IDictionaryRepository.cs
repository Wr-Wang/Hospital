using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IDictionaryRepository
{
    // 字典类型
    Task<DictionaryType?> GetTypeByIdAsync(long id);
    Task<DictionaryType?> GetTypeByCodeAsync(string code);
    Task<List<DictionaryType>> GetAllTypesAsync();
    Task AddTypeAsync(DictionaryType type);
    Task UpdateTypeAsync(DictionaryType type);
    Task DeleteTypeAsync(long id);

    // 字典项
    Task<DictionaryItem?> GetItemByIdAsync(long id);
    Task<List<DictionaryItem>> GetItemsByTypeIdAsync(long typeId);
    Task<List<DictionaryItem>> GetItemsByTypeCodeAsync(string typeCode);
    Task AddItemAsync(DictionaryItem item);
    Task UpdateItemAsync(DictionaryItem item);
    Task DeleteItemAsync(long id);
}
