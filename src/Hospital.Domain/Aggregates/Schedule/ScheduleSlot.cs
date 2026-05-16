using System;
using Hospital.Domain.Aggregates.Schedule;
using Hospital.Domain.Entities;

namespace Hospital.Domain.Aggregates.Schedule;

/// <summary>排班时段子实体，维护单个时段的配额和预约数</summary>
public class ScheduleSlot : Entity
{
    private static long _nextId;

    // EF Core
    private ScheduleSlot() { TimeSlot = default!; }

    public ScheduleSlot(TimeSlot timeSlot, int totalQuota)
    {
        Id = Interlocked.Increment(ref _nextId);
        TimeSlot = timeSlot ?? throw new ArgumentNullException(nameof(timeSlot));
        if (totalQuota <= 0)
            throw new ArgumentException("总配额必须大于 0", nameof(totalQuota));

        TotalQuota = totalQuota;
        BookedQuota = 0;
    }

    public TimeSlot TimeSlot { get; private set; }
    public int TotalQuota { get; private set; }
    public int BookedQuota { get; private set; }
    public int AvailableQuota => TotalQuota - BookedQuota;

    /// <summary>尝试预约，号源不足时返回 false</summary>
    public bool TryBook()
    {
        if (BookedQuota >= TotalQuota)
            return false;
        BookedQuota++;
        return true;
    }

    /// <summary>取消预约，恢复号源</summary>
    public void CancelBooking()
    {
        if (BookedQuota <= 0)
            throw new InvalidOperationException("无可取消的预约数");
        BookedQuota--;
    }

    /// <summary>更新总配额，不能小于已预约数</summary>
    public void UpdateQuota(int totalQuota)
    {
        if (totalQuota <= 0)
            throw new ArgumentException("总配额必须大于 0", nameof(totalQuota));
        if (totalQuota < BookedQuota)
            throw new InvalidOperationException("新配额不能小于已预约数");

        TotalQuota = totalQuota;
    }
}
