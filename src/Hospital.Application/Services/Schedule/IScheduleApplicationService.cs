using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IScheduleApplicationService
{
    Task<ScheduleDto?> GetByIdAsync(long id);
    Task<List<ScheduleDto>> GetByDoctorAsync(long doctorId);
    Task<List<ScheduleDto>> GetByDeptAsync(long deptId, string? date);
    Task<List<ScheduleDto>> GetAvailableAsync(long deptId, long? doctorId, string date);
    Task<long> CreateAsync(CreateScheduleDto dto);
    Task PublishAsync(long id);
    Task DeactivateAsync(long id);
    Task UpdateSlotQuotaAsync(long id, UpdateScheduleSlotDto dto);
}
