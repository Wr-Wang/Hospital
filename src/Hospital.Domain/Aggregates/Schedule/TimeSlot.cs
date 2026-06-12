using System;

namespace Hospital.Domain.Aggregates.Schedule;

/// <summary>时段值对象，包含时段名称和起止时间</summary>
public sealed record TimeSlot
{
    public string Name { get; }
    public TimeSpan StartTime { get; }
    public TimeSpan EndTime { get; }

    public TimeSlot(string name, TimeSpan startTime, TimeSpan endTime)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("时段名称不能为空", nameof(name));
        if (startTime >= endTime)
            throw new ArgumentException("开始时间必须早于结束时间", nameof(startTime));

        Name = name;
        StartTime = startTime;
        EndTime = endTime;
    }

    public override string ToString() => Name;
    public static implicit operator string(TimeSlot slot) => slot.Name;
}
