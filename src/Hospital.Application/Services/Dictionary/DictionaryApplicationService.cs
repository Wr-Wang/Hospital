using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Application.Services;

public sealed class DictionaryApplicationService : IDictionaryApplicationService
{
    private readonly IDictionaryRepository _repository;

    public DictionaryApplicationService(IDictionaryRepository repository)
    {
        _repository = repository;
    }

    // ===== 字典类型 =====

    public async Task<List<DictionaryTypeDto>> GetAllTypesAsync()
    {
        var types = await _repository.GetAllTypesAsync();
        return types.Select(MapTypeToDto).ToList();
    }

    public async Task<DictionaryTypeDto?> GetTypeByIdAsync(long id)
    {
        var type = await _repository.GetTypeByIdAsync(id);
        return MapTypeToDto(type);
    }

    public async Task<long> CreateTypeAsync(CreateDictionaryTypeDto dto)
    {
        var type = new DictionaryType(dto.Code, dto.Name, dto.Description);
        await _repository.AddTypeAsync(type);
        return type.Id;
    }

    public async Task UpdateTypeAsync(long id, UpdateDictionaryTypeDto dto)
    {
        var type = await _repository.GetTypeByIdAsync(id)
            ?? throw new InvalidOperationException($"字典类型不存在 (Id={id})");

        type.UpdateInfo(dto.Name, dto.Description);
        await _repository.UpdateTypeAsync(type);
    }

    public async Task ActivateTypeAsync(long id)
    {
        var type = await _repository.GetTypeByIdAsync(id)
            ?? throw new InvalidOperationException($"字典类型不存在 (Id={id})");

        type.Activate();
        await _repository.UpdateTypeAsync(type);
    }

    public async Task DeactivateTypeAsync(long id)
    {
        var type = await _repository.GetTypeByIdAsync(id)
            ?? throw new InvalidOperationException($"字典类型不存在 (Id={id})");

        type.Deactivate();
        await _repository.UpdateTypeAsync(type);
    }

    // ===== 字典项 =====

    public async Task<List<DictionaryItemDto>> GetItemsByTypeIdAsync(long typeId)
    {
        var items = await _repository.GetItemsByTypeIdAsync(typeId);
        return items.Select(MapItemToDto).ToList();
    }

    public async Task<List<DictionaryItemDto>> GetItemsByTypeCodeAsync(string typeCode)
    {
        var items = await _repository.GetItemsByTypeCodeAsync(typeCode);
        return items.Select(MapItemToDto).ToList();
    }

    public async Task<long> CreateItemAsync(CreateDictionaryItemDto dto)
    {
        var item = new DictionaryItem(dto.TypeId, dto.Code, dto.Name, dto.ParentId, dto.SortOrder);
        await _repository.AddItemAsync(item);
        return item.Id;
    }

    public async Task UpdateItemAsync(long id, UpdateDictionaryItemDto dto)
    {
        var item = await _repository.GetItemByIdAsync(id)
            ?? throw new InvalidOperationException($"字典项不存在 (Id={id})");

        item.UpdateInfo(dto.Name, dto.ParentId, dto.SortOrder);
        await _repository.UpdateItemAsync(item);
    }

    public async Task ActivateItemAsync(long id)
    {
        var item = await _repository.GetItemByIdAsync(id)
            ?? throw new InvalidOperationException($"字典项不存在 (Id={id})");

        item.Activate();
        await _repository.UpdateItemAsync(item);
    }

    public async Task DeactivateItemAsync(long id)
    {
        var item = await _repository.GetItemByIdAsync(id)
            ?? throw new InvalidOperationException($"字典项不存在 (Id={id})");

        item.Deactivate();
        await _repository.UpdateItemAsync(item);
    }

    // ===== Mapping =====

    private static DictionaryTypeDto MapTypeToDto(DictionaryType? type)
    {
        if (type is null) return null!;

        return new DictionaryTypeDto(type.Id, type.Code, type.Name, type.Description, type.IsActive);
    }

    private static DictionaryItemDto MapItemToDto(DictionaryItem? item)
    {
        if (item is null) return null!;

        return new DictionaryItemDto(
            item.Id, item.TypeId, item.Code, item.Name,
            item.ParentId, item.SortOrder, item.IsActive);
    }
}
