using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Domain.Aggregates.Schedule;

namespace Hospital.Application.Repositories;

public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(long id);
    Task<List<Schedule>> GetByDoctorAsync(long doctorId);
    Task<List<Schedule>> GetByDeptAsync(long deptId, DateOnly? date);
    Task<List<Schedule>> GetAvailableAsync(long deptId, long? doctorId, DateOnly date);
    Task AddAsync(Schedule schedule);
    Task UpdateAsync(Schedule schedule);
    Task DeleteAsync(long id);
}
