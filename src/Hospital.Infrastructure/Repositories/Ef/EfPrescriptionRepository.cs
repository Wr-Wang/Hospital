using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfPrescriptionRepository : IPrescriptionRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfPrescriptionRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Prescription?> GetByIdAsync(long id)
        => await _db.Prescriptions.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Prescription>> GetByEncounterIdAsync(long encounterId)
        => await _db.Prescriptions.Include(p => p.Items)
            .Where(p => p.EncounterId == encounterId)
            .ToListAsync();

    public async Task AddAsync(Prescription prescription)
    {
        await _db.Prescriptions.AddAsync(prescription);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Prescription prescription)
    {
        _db.Prescriptions.Update(prescription);
        await _db.SaveChangesAsync();
    }
}
