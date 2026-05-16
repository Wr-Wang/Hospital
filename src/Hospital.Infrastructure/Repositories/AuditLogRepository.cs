using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly List<AuditLog> _logs = new();

    public Task<AuditLog?> GetByIdAsync(long id)
        => Task.FromResult(_logs.FirstOrDefault(l => l.Id == id));

    public Task<List<AuditLog>> GetAllAsync()
        => Task.FromResult(_logs.OrderByDescending(l => l.Timestamp).ToList());

    public Task<List<AuditLog>> GetByUserIdAsync(long userId)
        => Task.FromResult(_logs.Where(l => l.UserId == userId).OrderByDescending(l => l.Timestamp).ToList());

    public Task<List<AuditLog>> GetByEntityAsync(string entityType, long entityId)
        => Task.FromResult(_logs.Where(l => l.EntityType == entityType && l.EntityId == entityId)
            .OrderByDescending(l => l.Timestamp).ToList());

    public Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to)
        => Task.FromResult(_logs.Where(l => l.Timestamp >= from && l.Timestamp <= to)
            .OrderByDescending(l => l.Timestamp).ToList());

    public Task AddAsync(AuditLog auditLog)
    {
        auditLog.GetType().GetProperty("Id")?.SetValue(auditLog, _logs.Count + 1);
        _logs.Add(auditLog);
        return Task.CompletedTask;
    }
}
