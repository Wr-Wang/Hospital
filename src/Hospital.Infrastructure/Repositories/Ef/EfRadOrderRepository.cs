using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfRadOrderRepository : IRadOrderRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfRadOrderRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<RadOrder?> GetByIdAsync(long id)
        => await _db.RadOrders.FindAsync(id);

    public async Task<List<RadOrder>> GetByEncounterIdAsync(long encounterId)
        => await _db.RadOrders.Where(o => o.EncounterId == encounterId).ToListAsync();

    public async Task AddAsync(RadOrder order)
    {
        await _db.RadOrders.AddAsync(order);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(RadOrder order)
    {
        _db.RadOrders.Update(order);
        await _db.SaveChangesAsync();
    }
}
