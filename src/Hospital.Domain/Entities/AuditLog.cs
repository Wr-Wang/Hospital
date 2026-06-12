namespace Hospital.Domain.Entities;

/// <summary>审计日志实体，记录关键操作的可追溯信息</summary>
public class AuditLog : Entity
{
    private AuditLog() { } // For EF Core

    public AuditLog(long userId, string userName, string action, string entityType,
        long entityId, string? oldValue, string? newValue, string? ipAddress)
    {
        UserId = userId;
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Action = action ?? throw new ArgumentNullException(nameof(action));
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        EntityId = entityId;
        OldValue = oldValue;
        NewValue = newValue;
        IpAddress = ipAddress;
        Timestamp = DateTime.Now;
    }

    public long UserId { get; private set; }
    public string UserName { get; private set; }
    public string Action { get; private set; }   // Create / Update / Delete / Login / Dispense / Pay / Refund
    public string EntityType { get; private set; } // Patient / Prescription / Billing / User / Role
    public long EntityId { get; private set; }
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime Timestamp { get; private set; }
}
