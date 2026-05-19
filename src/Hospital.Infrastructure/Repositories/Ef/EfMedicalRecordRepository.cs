using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfMedicalRecordRepository : IMedicalRecordRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfMedicalRecordRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<MedicalRecord?> GetByEncounterIdAsync(long encounterId)
        => await _db.MedicalRecords.FirstOrDefaultAsync(r => r.EncounterId == encounterId);

    public async Task AddAsync(MedicalRecord record)
    {
        await _db.MedicalRecords.AddAsync(record);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(MedicalRecord record)
    {
        _db.MedicalRecords.Update(record);
        await _db.SaveChangesAsync();
    }
}
