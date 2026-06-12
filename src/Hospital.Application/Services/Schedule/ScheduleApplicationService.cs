using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Aggregates.Schedule;

namespace Hospital.Application.Services;

public sealed class ScheduleApplicationService : IScheduleApplicationService
{
    private readonly IScheduleRepository _repository;

    public ScheduleApplicationService(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScheduleDto?> GetByIdAsync(long id)
    {
        var schedule = await _repository.GetByIdAsync(id);
        return MapToDto(schedule);
    }

    public async Task<List<ScheduleDto>> GetByDoctorAsync(long doctorId)
    {
        var list = await _repository.GetByDoctorAsync(doctorId);
        return list.Select(s => MapToDto(s)!).ToList();
    }

    public async Task<List<ScheduleDto>> GetByDeptAsync(long deptId, string? date)
    {
        DateOnly? parsed = date is not null ? DateOnly.Parse(date) : null;
        var list = await _repository.GetByDeptAsync(deptId, parsed);
        return list.Select(s => MapToDto(s)!).ToList();
    }

    public async Task<List<ScheduleDto>> GetAvailableAsync(long deptId, long? doctorId, string date)
    {
        var parsed = DateOnly.Parse(date);
        var list = await _repository.GetAvailableAsync(deptId, doctorId, parsed);
        return list.Select(s => MapToDto(s)!).ToList();
    }

    public async Task<long> CreateAsync(CreateScheduleDto dto)
    {
        var scheduleDate = DateOnly.Parse(dto.ScheduleDate);
        var slots = dto.Slots.Select(s => new ScheduleSlot(
            new TimeSlot(s.SlotName, TimeSpan.Parse(s.StartTime), TimeSpan.Parse(s.EndTime)),
            s.TotalQuota)).ToList();

        var schedule = new Domain.Aggregates.Schedule.Schedule(
            dto.DoctorId, dto.DeptId, dto.CampusId, scheduleDate, slots);

        await _repository.AddAsync(schedule);
        return schedule.Id;
    }

    public async Task PublishAsync(long id)
    {
        var schedule = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"排班不存在 (Id={id})");
        schedule.Publish();
        await _repository.UpdateAsync(schedule);
    }

    public async Task DeactivateAsync(long id)
    {
        var schedule = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"排班不存在 (Id={id})");
        schedule.Deactivate();
        await _repository.UpdateAsync(schedule);
    }

    public async Task UpdateSlotQuotaAsync(long id, UpdateScheduleSlotDto dto)
    {
        var schedule = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"排班不存在 (Id={id})");

        var slot = schedule.GetSlot(dto.SlotName)
            ?? throw new InvalidOperationException($"时段 '{dto.SlotName}' 不存在");

        slot.UpdateQuota(dto.TotalQuota);
        await _repository.UpdateAsync(schedule);
    }

    private static ScheduleDto? MapToDto(Domain.Aggregates.Schedule.Schedule? s)
    {
        if (s is null) return null;

        return new ScheduleDto(
            s.Id, s.DoctorId, s.DeptId, s.CampusId,
            s.ScheduleDate.ToString("yyyy-MM-dd"),
            s.Status.ToString(),
            s.Slots.Select(sl => new ScheduleSlotDto(
                sl.Id,
                sl.TimeSlot.Name,
                sl.TimeSlot.StartTime.ToString(@"hh\:mm"),
                sl.TimeSlot.EndTime.ToString(@"hh\:mm"),
                sl.TotalQuota,
                sl.BookedQuota,
                sl.AvailableQuota)).ToList());
    }
}
