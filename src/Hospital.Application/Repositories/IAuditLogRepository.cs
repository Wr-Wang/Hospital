using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(long id);
    Task<List<AuditLog>> GetAllAsync();
    Task<List<AuditLog>> GetByUserIdAsync(long userId);
    Task<List<AuditLog>> GetByEntityAsync(string entityType, long entityId);
    Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task AddAsync(AuditLog auditLog);
}
