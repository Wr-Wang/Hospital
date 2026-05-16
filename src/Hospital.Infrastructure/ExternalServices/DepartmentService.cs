using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>科室模块 HTTP 服务实现（WPF 端）</summary>
public sealed class DepartmentService : IDepartmentApplicationService
{
    private readonly IApiClient _api;

    public DepartmentService(IApiClient api) => _api = api;

    public async Task<List<DepartmentDto>> GetAllAsync()
        => await _api.GetAsync<List<DepartmentDto>>("Department");

    public async Task<List<DepartmentDto>> GetTreeByCampusIdAsync(long campusId)
        => await _api.GetAsync<List<DepartmentDto>>($"Department/tree/{campusId}");

    public async Task<DepartmentDto?> GetByIdAsync(long id)
        => await _api.GetAsyncOrDefault<DepartmentDto>($"Department/{id}");

    public async Task<long> CreateAsync(CreateDepartmentDto dto)
    {
        var result = await _api.PostAsync<IdResponse>("Department", dto);
        return result.Id;
    }

    public async Task UpdateAsync(long id, UpdateDepartmentDto dto)
        => await _api.PutAsync($"Department/{id}", dto);

    public async Task ActivateAsync(long id)
        => await _api.PatchAsync($"Department/{id}/activate");

    public async Task DeactivateAsync(long id)
        => await _api.PatchAsync($"Department/{id}/deactivate");

    private sealed record IdResponse(long Id);
}
