using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Application.Services;

public sealed class RegistrationApplicationService : IRegistrationApplicationService
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IEncounterRepository _encounterRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public RegistrationApplicationService(
        IRegistrationRepository registrationRepository,
        IEncounterRepository encounterRepository,
        IScheduleRepository scheduleRepository)
    {
        _registrationRepository = registrationRepository;
        _encounterRepository = encounterRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<RegistrationDto?> GetByIdAsync(long id)
    {
        var reg = await _registrationRepository.GetByIdAsync(id);
        return MapToDto(reg);
    }

    public async Task<List<RegistrationDto>> GetByPatientAsync(long patientId)
    {
        var list = await _registrationRepository.GetByPatientAsync(patientId);
        return list.Select(r => MapToDto(r)!).ToList();
    }

    public async Task<List<RegistrationDto>> GetByDoctorAsync(long doctorId, string? date)
    {
        DateOnly? parsed = date is not null ? DateOnly.Parse(date) : null;
        var list = await _registrationRepository.GetByDoctorAsync(doctorId, parsed);
        return list.Select(r => MapToDto(r)!).ToList();
    }

    /// <summary>挂号流程：校验号源 → 扣减号源 → 创建挂号记录 → 创建就诊记录</summary>
    public async Task<long> RegisterAsync(CreateRegistrationDto dto)
    {
        // 1. 加载排班并校验号源
        var schedule = await _scheduleRepository.GetByIdAsync(dto.ScheduleId)
            ?? throw new InvalidOperationException($"排班不存在 (Id={dto.ScheduleId})");

        if (!schedule.TryBookSlot(dto.SlotName))
            throw new InvalidOperationException("该时段号源已满或排班不可用");

        // 2. 生成排队号
        var queueNumber = await _registrationRepository.GetNextQueueNumberAsync(dto.ScheduleId, dto.SlotName);

        // 3. 创建挂号记录
        var registration = new Registration(
            dto.PatientId, dto.ScheduleId, dto.DoctorId, dto.DeptId,
            dto.CampusId, queueNumber, dto.SlotName);

        // 4. 创建就诊记录
        var encounter = new Encounter(
            dto.PatientId, dto.DoctorId, dto.DeptId, dto.CampusId, registration.Id);

        // 5. 持久化
        await _scheduleRepository.UpdateAsync(schedule);
        await _registrationRepository.AddAsync(registration);
        await _encounterRepository.AddAsync(encounter);

        return registration.Id;
    }

    /// <summary>退号流程：校验状态 → 恢复号源 → 标记退号</summary>
    public async Task VoidAsync(long id)
    {
        var registration = await _registrationRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"挂号记录不存在 (Id={id})");

        registration.Void();

        var schedule = await _scheduleRepository.GetByIdAsync(registration.ScheduleId)
            ?? throw new InvalidOperationException($"排班不存在 (Id={registration.ScheduleId})");

        schedule.CancelBooking(registration.SlotName);

        await _registrationRepository.UpdateAsync(registration);
        await _scheduleRepository.UpdateAsync(schedule);
    }

    private static RegistrationDto? MapToDto(Registration? r)
    {
        if (r is null) return null;

        return new RegistrationDto(
            r.Id, r.PatientId, r.ScheduleId, r.DoctorId, r.DeptId,
            r.CampusId, r.RegisterTime.ToString("yyyy-MM-dd HH:mm:ss"),
            r.QueueNumber, r.SlotName, r.Status.ToString());
    }
}
