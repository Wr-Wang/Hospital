using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain.Aggregates.Schedule;
using Hospital.Domain.Enums;

namespace Hospital.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly List<Schedule> _schedules = new();

    public Task<Schedule?> GetByIdAsync(long id)
        => Task.FromResult(_schedules.FirstOrDefault(s => s.Id == id));

    public Task<List<Schedule>> GetByDoctorAsync(long doctorId)
        => Task.FromResult(_schedules.Where(s => s.DoctorId == doctorId).ToList());

    public Task<List<Schedule>> GetByDeptAsync(long deptId, DateOnly? date)
    {
        var query = _schedules.Where(s => s.DeptId == deptId);
        if (date.HasValue)
            query = query.Where(s => s.ScheduleDate == date.Value);
        return Task.FromResult(query.ToList());
    }

    public Task<List<Schedule>> GetAvailableAsync(long deptId, long? doctorId, DateOnly date)
    {
        var query = _schedules.Where(s =>
            s.DeptId == deptId &&
            s.ScheduleDate == date &&
            s.Status == ScheduleStatus.已发布 &&
            s.Slots.Any(sl => sl.AvailableQuota > 0));

        if (doctorId.HasValue)
            query = query.Where(s => s.DoctorId == doctorId.Value);

        return Task.FromResult(query.ToList());
    }

    public Task AddAsync(Schedule schedule)
    {
        schedule.GetType().GetProperty("Id")?.SetValue(schedule, _schedules.Count + 1);
        _schedules.Add(schedule);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Schedule schedule)
    {
        var index = _schedules.FindIndex(s => s.Id == schedule.Id);
        if (index >= 0)
            _schedules[index] = schedule;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long id)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == id);
        if (schedule is not null)
            _schedules.Remove(schedule);
        return Task.CompletedTask;
    }
}
