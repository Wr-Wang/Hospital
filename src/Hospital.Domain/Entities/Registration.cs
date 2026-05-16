using System;
using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities;

/// <summary>挂号记录实体，跟踪一次挂号的生命周期（已挂号→已就诊/已退号）</summary>
public class Registration : Entity
{
    // EF Core
    private Registration() { SlotName = default!; }

    public Registration(long patientId, long scheduleId, long doctorId, long deptId,
        long campusId, int queueNumber, string slotName)
    {
        if (string.IsNullOrWhiteSpace(slotName))
            throw new ArgumentException("时段名称不能为空", nameof(slotName));

        PatientId = patientId;
        ScheduleId = scheduleId;
        DoctorId = doctorId;
        DeptId = deptId;
        CampusId = campusId;
        RegisterTime = DateTime.Now;
        QueueNumber = queueNumber;
        SlotName = slotName;
        Status = RegistrationStatus.已挂号;
    }

    public long PatientId { get; private set; }
    public long ScheduleId { get; private set; }
    public long DoctorId { get; private set; }
    public long DeptId { get; private set; }
    public long CampusId { get; private set; }
    public DateTime RegisterTime { get; private set; }
    public int QueueNumber { get; private set; }
    public string SlotName { get; private set; }
    public RegistrationStatus Status { get; private set; }

    /// <summary>标记为已就诊</summary>
    public void MarkVisited()
    {
        if (Status != RegistrationStatus.已挂号)
            throw new InvalidOperationException("只有已挂号状态才能标记为已就诊");
        Status = RegistrationStatus.已就诊;
    }

    /// <summary>退号</summary>
    public void Void()
    {
        if (Status != RegistrationStatus.已挂号)
            throw new InvalidOperationException("只有已挂号状态才能退号");
        Status = RegistrationStatus.已退号;
    }
}
