using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IDictionaryApplicationService
{
    // 字典类型
    Task<List<DictionaryTypeDto>> GetAllTypesAsync();
    Task<DictionaryTypeDto?> GetTypeByIdAsync(long id);
    Task<long> CreateTypeAsync(CreateDictionaryTypeDto dto);
    Task UpdateTypeAsync(long id, UpdateDictionaryTypeDto dto);
    Task ActivateTypeAsync(long id);
    Task DeactivateTypeAsync(long id);

    // 字典项
    Task<List<DictionaryItemDto>> GetItemsByTypeIdAsync(long typeId);
    Task<List<DictionaryItemDto>> GetItemsByTypeCodeAsync(string typeCode);
    Task<long> CreateItemAsync(CreateDictionaryItemDto dto);
    Task UpdateItemAsync(long id, UpdateDictionaryItemDto dto);
    Task ActivateItemAsync(long id);
    Task DeactivateItemAsync(long id);
}
