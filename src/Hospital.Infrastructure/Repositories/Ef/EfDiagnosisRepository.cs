using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfDiagnosisRepository : IDiagnosisRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfDiagnosisRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<List<Diagnosis>> GetByEncounterIdAsync(long encounterId)
        => await _db.Diagnoses.Where(d => d.EncounterId == encounterId).ToListAsync();

    public async Task AddAsync(Diagnosis diagnosis)
    {
        await _db.Diagnoses.AddAsync(diagnosis);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Diagnosis diagnosis)
    {
        _db.Diagnoses.Update(diagnosis);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(long id)
    {
        var entity = await _db.Diagnoses.FindAsync(id);
        if (entity is not null)
        {
            _db.Diagnoses.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
