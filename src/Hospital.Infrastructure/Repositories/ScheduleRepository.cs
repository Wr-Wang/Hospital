using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain;
using Hospital.Domain.Aggregates.Schedule;
using Hospital.Domain.Enums;

namespace Hospital.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly List<Schedule> _schedules = new();

    public ScheduleRepository()
    {
        // 医生 ID: 1=张三(内科), 2=李四(外科), 3=王医生(儿科)
        // 科室 ID: 1=内科, 2=外科, 4=儿科
        var today = DateOnly.FromDateTime(DateTime.Now);

        var seeds = new List<(long doctorId, long deptId, long campusId, DateOnly date, (string name, string start, string end, int quota)[] slots)>
        {
            (1, 1, 1, today.AddDays(2), new[] { ("上午", "08:00", "12:00", 30), ("下午", "13:00", "17:00", 20) }),
            (2, 2, 1, today.AddDays(2), new[] { ("上午", "08:00", "12:00", 25) }),
            (3, 4, 1, today.AddDays(3), new[] { ("上午", "08:00", "12:00", 20), ("下午", "13:00", "17:00", 15) }),
            (1, 1, 1, today.AddDays(3), new[] { ("上午", "08:00", "12:00", 30) }),
            (2, 2, 1, today.AddDays(4), new[] { ("上午", "08:00", "12:00", 25), ("下午", "13:00", "17:00", 20) }),
            (1, 1, 1, today.AddDays(7), new[] { ("上午", "08:00", "12:00", 30), ("下午", "13:00", "17:00", 20) }),
            (3, 4, 1, today.AddDays(7), new[] { ("上午", "08:00", "12:00", 20) }),
        };

        foreach (var (doctorId, deptId, campusId, date, slotDefs) in seeds)
        {
            var slots = slotDefs.Select(s => new ScheduleSlot(
                new TimeSlot(s.name, TimeSpan.Parse(s.start), TimeSpan.Parse(s.end)),
                s.quota)).ToList();

            var schedule = new Schedule(doctorId, deptId, campusId, date, slots);
            typeof(Entity).GetProperty("Id")?.SetValue(schedule, _schedules.Count + 1);
            _schedules.Add(schedule);
        }
    }

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
