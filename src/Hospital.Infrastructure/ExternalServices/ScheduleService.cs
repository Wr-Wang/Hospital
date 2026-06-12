using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>排班模块 HTTP 服务实现，调用后端 ScheduleController 接口</summary>
public sealed class ScheduleService : IScheduleApplicationService
{
    private readonly IApiClient _api;

    public ScheduleService(IApiClient api) => _api = api;

    public async Task<ScheduleDto?> GetByIdAsync(long id)
        => await _api.GetAsyncOrDefault<ScheduleDto>(ApiRoutes.Schedule.ById(id));

    public async Task<List<ScheduleDto>> GetByDoctorAsync(long doctorId)
        => await _api.GetAsync<List<ScheduleDto>>(ApiRoutes.Schedule.ByDoctor(doctorId));

    public async Task<List<ScheduleDto>> GetByDeptAsync(long deptId, string? date)
        => await _api.GetAsync<List<ScheduleDto>>(ApiRoutes.Schedule.ByDept(deptId, date));

    public async Task<List<ScheduleDto>> GetAvailableAsync(long deptId, long? doctorId, string date)
        => await _api.GetAsync<List<ScheduleDto>>(ApiRoutes.Schedule.Available(deptId, doctorId, date));

    public async Task<long> CreateAsync(CreateScheduleDto request)
    {
        var result = await _api.PostAsync<IdResponse>(ApiRoutes.Schedule.Base, request);
        return result.Id;
    }

    public async Task PublishAsync(long id)
        => await _api.PatchAsync(ApiRoutes.Schedule.Publish(id));

    public async Task DeactivateAsync(long id)
        => await _api.PatchAsync(ApiRoutes.Schedule.Deactivate(id));

    public async Task UpdateSlotQuotaAsync(long id, UpdateScheduleSlotDto dto)
        => await _api.PutAsync(ApiRoutes.Schedule.SlotQuota(id), dto);

    private sealed record IdResponse(long Id);
}
