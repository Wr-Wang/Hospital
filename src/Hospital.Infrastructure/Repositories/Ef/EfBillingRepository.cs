using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfBillingRepository : IBillingRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfBillingRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Billing?> GetByIdAsync(long id)
        => await _db.Billings.Include(b => b.Items).Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<List<Billing>> GetByPatientIdAsync(long patientId)
        => await _db.Billings.Include(b => b.Items).Include(b => b.Payments)
            .Where(b => b.PatientId == patientId)
            .ToListAsync();

    public async Task<List<Billing>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _db.Billings.Include(b => b.Items).Include(b => b.Payments)
            .Where(b => b.CreatedAt >= from && b.CreatedAt <= to)
            .ToListAsync();

    public async Task AddAsync(Billing billing)
    {
        await _db.Billings.AddAsync(billing);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Billing billing)
    {
        _db.Billings.Update(billing);
        await _db.SaveChangesAsync();
    }
}
