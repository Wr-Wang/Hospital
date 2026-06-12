using System;
using System.Collections.Generic;
using System.Linq;
using Hospital.Domain.Enums;
using Hospital.Domain.Aggregates.Schedule;

namespace Hospital.Domain.Aggregates.Schedule;

/// <summary>排班聚合根，管理医生在特定日期的出诊时段和号源</summary>
public class Schedule : AggregateRoot
{
    private readonly List<ScheduleSlot> _slots = new();

    // EF Core
    private Schedule() { }

    public Schedule(long doctorId, long deptId, long campusId, DateOnly scheduleDate, List<ScheduleSlot> slots)
    {
        if (slots is null || slots.Count == 0)
            throw new ArgumentException("排班至少需要一个时段", nameof(slots));
        if (slots.Select(s => s.TimeSlot.Name).Distinct().Count() != slots.Count)
            throw new ArgumentException("时段名称不能重复", nameof(slots));

        DoctorId = doctorId;
        DeptId = deptId;
        CampusId = campusId;
        ScheduleDate = scheduleDate;
        _slots.AddRange(slots);
        Status = ScheduleStatus.已发布;
    }

    public long DoctorId { get; private set; }
    public long DeptId { get; private set; }
    public long CampusId { get; private set; }
    public DateOnly ScheduleDate { get; private set; }
    public ScheduleStatus Status { get; private set; }
    public IReadOnlyCollection<ScheduleSlot> Slots => _slots.AsReadOnly();

    /// <summary>发布排班</summary>
    public void Publish()
    {
        if (Status == ScheduleStatus.已停用)
            throw new InvalidOperationException("已停用的排班无法发布");
        Status = ScheduleStatus.已发布;
    }

    /// <summary>停用排班</summary>
    public void Deactivate() => Status = ScheduleStatus.已停用;

    /// <summary>尝试预约指定时段，返回是否成功</summary>
    public bool TryBookSlot(string slotName)
    {
        if (Status != ScheduleStatus.已发布)
            return false;

        var slot = _slots.FirstOrDefault(s => s.TimeSlot.Name == slotName);
        if (slot?.TryBook() != true)
            return false;

        if (_slots.All(s => s.AvailableQuota == 0))
            Status = ScheduleStatus.已满;

        return true;
    }

    /// <summary>取消指定时段的预约，恢复号源</summary>
    public void CancelBooking(string slotName)
    {
        var slot = _slots.FirstOrDefault(s => s.TimeSlot.Name == slotName)
            ?? throw new InvalidOperationException($"时段 '{slotName}' 不存在");

        slot.CancelBooking();

        // 从已满状态恢复为已发布
        if (Status == ScheduleStatus.已满)
            Status = ScheduleStatus.已发布;
    }

    /// <summary>获取指定时段</summary>
    public ScheduleSlot? GetSlot(string slotName)
        => _slots.FirstOrDefault(s => s.TimeSlot.Name == slotName);
}
