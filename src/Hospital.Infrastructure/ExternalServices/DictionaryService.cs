using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>字典模块 HTTP 服务实现（WPF 端）</summary>
public sealed class DictionaryService : IDictionaryApplicationService
{
    private readonly IApiClient _api;

    public DictionaryService(IApiClient api) => _api = api;

    // ===== 字典类型 =====

    public async Task<List<DictionaryTypeDto>> GetAllTypesAsync()
        => await _api.GetAsync<List<DictionaryTypeDto>>("Dictionary/types");

    public async Task<DictionaryTypeDto?> GetTypeByIdAsync(long id)
        => await _api.GetAsyncOrDefault<DictionaryTypeDto>($"Dictionary/types/{id}");

    public async Task<long> CreateTypeAsync(CreateDictionaryTypeDto dto)
    {
        var result = await _api.PostAsync<IdResponse>("Dictionary/types", dto);
        return result.Id;
    }

    public async Task UpdateTypeAsync(long id, UpdateDictionaryTypeDto dto)
        => await _api.PutAsync($"Dictionary/types/{id}", dto);

    public async Task ActivateTypeAsync(long id)
        => await _api.PatchAsync($"Dictionary/types/{id}/activate");

    public async Task DeactivateTypeAsync(long id)
        => await _api.PatchAsync($"Dictionary/types/{id}/deactivate");

    // ===== 字典项 =====

    public async Task<List<DictionaryItemDto>> GetItemsByTypeIdAsync(long typeId)
        => await _api.GetAsync<List<DictionaryItemDto>>($"Dictionary/items/by-type/{typeId}");

    public async Task<List<DictionaryItemDto>> GetItemsByTypeCodeAsync(string typeCode)
        => await _api.GetAsync<List<DictionaryItemDto>>($"Dictionary/items/by-code/{typeCode}");

    public async Task<long> CreateItemAsync(CreateDictionaryItemDto dto)
    {
        var result = await _api.PostAsync<IdResponse>("Dictionary/items", dto);
        return result.Id;
    }

    public async Task UpdateItemAsync(long id, UpdateDictionaryItemDto dto)
        => await _api.PutAsync($"Dictionary/items/{id}", dto);

    public async Task ActivateItemAsync(long id)
        => await _api.PatchAsync($"Dictionary/items/{id}/activate");

    public async Task DeactivateItemAsync(long id)
        => await _api.PatchAsync($"Dictionary/items/{id}/deactivate");

    private sealed record IdResponse(long Id);
}
