using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>人员模块 HTTP 服务实现（WPF 端），只实现排班/挂号页面需要的方法</summary>
public sealed class StaffService : IStaffApplicationService
{
    private readonly IApiClient _api;

    public StaffService(IApiClient api) => _api = api;

    public async Task<List<StaffDto>> GetAllAsync()
        => await _api.GetAsync<List<StaffDto>>("Staff");

    public async Task<List<StaffDto>> GetByDeptIdAsync(long deptId)
        => await _api.GetAsync<List<StaffDto>>($"Staff/by-dept/{deptId}");

    public Task<StaffDto?> GetByIdAsync(long id) => throw new NotSupportedException();
    public Task<List<StaffDto>> GetByCampusIdAsync(long campusId) => throw new NotSupportedException();
    public Task<long> CreateAsync(CreateStaffDto dto) => throw new NotSupportedException();
    public Task UpdateAsync(long id, UpdateStaffDto dto) => throw new NotSupportedException();
    public Task UpdateLicenseAsync(long id, UpdateStaffLicenseDto dto) => throw new NotSupportedException();
    public Task ActivateAsync(long id) => throw new NotSupportedException();
    public Task DeactivateAsync(long id) => throw new NotSupportedException();
}
