using System.Collections.Generic;

namespace Hospital.Application.DTOs;

// ===== 排班号表（Schedule） =====

public sealed record ScheduleDto(
    long Id,
    long DoctorId,
    long DeptId,
    long CampusId,
    string ScheduleDate,
    string Status,
    List<ScheduleSlotDto> Slots);

public sealed record ScheduleSlotDto(
    long Id,
    string SlotName,
    string StartTime,
    string EndTime,
    int TotalQuota,
    int BookedQuota,
    int AvailableQuota);

public sealed record CreateScheduleDto(
    long DoctorId,
    long DeptId,
    long CampusId,
    string ScheduleDate,
    List<CreateScheduleSlotDto> Slots);

public sealed record CreateScheduleSlotDto(
    string SlotName,
    string StartTime,
    string EndTime,
    int TotalQuota);

public sealed record UpdateScheduleSlotDto(
    string SlotName,
    int TotalQuota);

// ===== 挂号工作台（Registration） =====

public sealed record RegistrationDto(
    long Id,
    long PatientId,
    long ScheduleId,
    long DoctorId,
    long DeptId,
    long CampusId,
    string RegisterTime,
    int QueueNumber,
    string SlotName,
    string Status);

public sealed record CreateRegistrationDto(
    long PatientId,
    long ScheduleId,
    long DoctorId,
    long DeptId,
    long CampusId,
    string SlotName);
