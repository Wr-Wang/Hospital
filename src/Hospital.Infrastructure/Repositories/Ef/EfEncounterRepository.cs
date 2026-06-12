using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfEncounterRepository : IEncounterRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfEncounterRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Encounter?> GetByIdAsync(long id)
        => await _db.Encounters.FindAsync(id);

    public async Task<Encounter?> GetByRegistrationIdAsync(long registrationId)
        => await _db.Encounters.FirstOrDefaultAsync(e => e.RegistrationId == registrationId);

    public async Task<List<Encounter>> GetByDoctorAsync(long doctorId)
        => await _db.Encounters.Where(e => e.DoctorId == doctorId).ToListAsync();

    public async Task<List<Encounter>> GetByPatientAsync(long patientId)
        => await _db.Encounters.Where(e => e.PatientId == patientId).ToListAsync();

    public async Task<List<Encounter>> GetByDateAsync(long doctorId, DateOnly date)
    {
        var from = date.ToDateTime(TimeOnly.MinValue);
        var to = date.ToDateTime(TimeOnly.MaxValue);
        return await _db.Encounters
            .Where(e => e.DoctorId == doctorId && e.StartTime >= from && e.StartTime <= to)
            .ToListAsync();
    }

    public async Task<List<Encounter>> GetQueueAsync(long doctorId, DateOnly date)
    {
        var from = date.ToDateTime(TimeOnly.MinValue);
        var to = date.ToDateTime(TimeOnly.MaxValue);
        return await _db.Encounters
            .Where(e => e.DoctorId == doctorId && e.StartTime >= from && e.StartTime <= to)
            .OrderBy(e => e.Id)
            .ToListAsync();
    }

    public async Task AddAsync(Encounter encounter)
    {
        await _db.Encounters.AddAsync(encounter);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Encounter encounter)
    {
        _db.Encounters.Update(encounter);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Encounters.FindAsync(id);
        if (entity is not null)
        {
            _db.Encounters.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
