using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>科室模块 HTTP 服务实现（WPF 端），只实现排班/挂号页面需要的方法</summary>
public sealed class DepartmentService : IDepartmentApplicationService
{
    private readonly IApiClient _api;

    public DepartmentService(IApiClient api) => _api = api;

    public async Task<List<DepartmentDto>> GetAllAsync()
        => await _api.GetAsync<List<DepartmentDto>>("Department");

    public Task<DepartmentDto?> GetByIdAsync(long id) => throw new NotSupportedException();
    public Task<List<DepartmentDto>> GetTreeByCampusIdAsync(long campusId) => throw new NotSupportedException();
    public Task<long> CreateAsync(CreateDepartmentDto dto) => throw new NotSupportedException();
    public Task UpdateAsync(long id, UpdateDepartmentDto dto) => throw new NotSupportedException();
    public Task ActivateAsync(long id) => throw new NotSupportedException();
    public Task DeactivateAsync(long id) => throw new NotSupportedException();
}
