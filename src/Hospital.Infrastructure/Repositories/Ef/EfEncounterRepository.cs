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
        // 与 GetQueueAsync 一致：按挂号时间（RegisterTime）过滤当天，包含待诊记录
        var from = date.ToDateTime(TimeOnly.MinValue);
        var to = from.AddDays(1);
        return await _db.Encounters
            .Where(e => e.DoctorId == doctorId
                && _db.Registrations.Any(r => r.Id == e.RegistrationId
                    && r.RegisterTime >= from
                    && r.RegisterTime < to))
            .ToListAsync();
    }

    public async Task<List<Encounter>> GetQueueAsync(long doctorId, DateOnly date)
    {
        // 队列按挂号时间（RegisterTime）过滤当天，而非 StartTime：
        // 待诊记录尚未开始就诊，StartTime 为 null，按 StartTime 过滤会漏掉待诊患者。
        var from = date.ToDateTime(TimeOnly.MinValue);
        var to = from.AddDays(1);
        return await _db.Encounters
            .Where(e => e.DoctorId == doctorId
                && _db.Registrations.Any(r => r.Id == e.RegistrationId
                    && r.RegisterTime >= from
                    && r.RegisterTime < to))
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
