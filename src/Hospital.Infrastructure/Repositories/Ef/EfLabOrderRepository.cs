using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfLabOrderRepository : ILabOrderRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfLabOrderRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<LabOrder?> GetByIdAsync(long id)
        => await _db.LabOrders.FindAsync(id);

    public async Task<List<LabOrder>> GetByEncounterIdAsync(long encounterId)
        => await _db.LabOrders.Where(o => o.EncounterId == encounterId).ToListAsync();

    public async Task AddAsync(LabOrder order)
    {
        await _db.LabOrders.AddAsync(order);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(LabOrder order)
    {
        _db.LabOrders.Update(order);
        await _db.SaveChangesAsync();
    }
}
