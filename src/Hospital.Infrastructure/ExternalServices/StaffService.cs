using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>人员模块 HTTP 服务实现（WPF 端）</summary>
public sealed class StaffService : IStaffApplicationService
{
    private readonly IApiClient _api;

    public StaffService(IApiClient api) => _api = api;

    public async Task<List<StaffDto>> GetAllAsync()
        => await _api.GetAsync<List<StaffDto>>("Staff");

    public async Task<List<StaffDto>> GetByCampusIdAsync(long campusId)
        => await _api.GetAsync<List<StaffDto>>($"Staff/by-campus/{campusId}");

    public async Task<List<StaffDto>> GetByDeptIdAsync(long deptId)
        => await _api.GetAsync<List<StaffDto>>($"Staff/by-dept/{deptId}");

    public async Task<StaffDto?> GetByIdAsync(long id)
        => await _api.GetAsyncOrDefault<StaffDto>($"Staff/{id}");

    public async Task<long> CreateAsync(CreateStaffDto dto)
    {
        var result = await _api.PostAsync<IdResponse>("Staff", dto);
        return result.Id;
    }

    public async Task UpdateAsync(long id, UpdateStaffDto dto)
        => await _api.PutAsync($"Staff/{id}", dto);

    public async Task UpdateLicenseAsync(long id, UpdateStaffLicenseDto dto)
        => await _api.PatchAsync($"Staff/{id}/license", (object)dto);

    public async Task ActivateAsync(long id)
        => await _api.PatchAsync($"Staff/{id}/activate");

    public async Task DeactivateAsync(long id)
        => await _api.PatchAsync($"Staff/{id}/deactivate");

    private sealed record IdResponse(long Id);
}
