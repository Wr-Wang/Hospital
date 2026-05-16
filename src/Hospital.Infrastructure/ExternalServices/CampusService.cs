using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>院区模块 HTTP 服务实现（WPF 端）</summary>
public sealed class CampusService : ICampusApplicationService
{
    private readonly IApiClient _api;

    public CampusService(IApiClient api) => _api = api;

    public async Task<List<CampusDto>> GetAllAsync()
        => await _api.GetAsync<List<CampusDto>>("Campus");

    public async Task<List<CampusDto>> GetActiveAsync()
        => await _api.GetAsync<List<CampusDto>>("Campus/active");

    public async Task<CampusDto?> GetByIdAsync(long id)
        => await _api.GetAsyncOrDefault<CampusDto>($"Campus/{id}");

    public async Task<long> CreateAsync(CreateCampusDto dto)
    {
        var result = await _api.PostAsync<IdResponse>("Campus", dto);
        return result.Id;
    }

    public async Task UpdateAsync(long id, UpdateCampusDto dto)
        => await _api.PutAsync($"Campus/{id}", dto);

    public async Task ActivateAsync(long id)
        => await _api.PatchAsync($"Campus/{id}/activate");

    public async Task DeactivateAsync(long id)
        => await _api.PatchAsync($"Campus/{id}/deactivate");

    private sealed record IdResponse(long Id);
}
