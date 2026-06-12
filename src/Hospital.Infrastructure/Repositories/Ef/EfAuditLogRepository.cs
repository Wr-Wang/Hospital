using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfAuditLogRepository : IAuditLogRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfAuditLogRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<AuditLog?> GetByIdAsync(long id)
        => await _db.AuditLogs.FindAsync(id);

    public async Task<List<AuditLog>> GetAllAsync()
        => await _db.AuditLogs.OrderByDescending(a => a.Timestamp).ToListAsync();

    public async Task<List<AuditLog>> GetByUserIdAsync(long userId)
        => await _db.AuditLogs.Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

    public async Task<List<AuditLog>> GetByEntityAsync(string entityType, long entityId)
        => await _db.AuditLogs
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

    public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _db.AuditLogs
            .Where(a => a.Timestamp >= from && a.Timestamp <= to)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

    public async Task AddAsync(AuditLog auditLog)
    {
        await _db.AuditLogs.AddAsync(auditLog);
        await _db.SaveChangesAsync();
    }
}
