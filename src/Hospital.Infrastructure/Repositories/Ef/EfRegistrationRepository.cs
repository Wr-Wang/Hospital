using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfRegistrationRepository : IRegistrationRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfRegistrationRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Registration?> GetByIdAsync(long id)
        => await _db.Registrations.FindAsync(id);

    public async Task<List<Registration>> GetByPatientAsync(long patientId)
        => await _db.Registrations
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.RegisterTime)
            .ToListAsync();

    public async Task<List<Registration>> GetByDoctorAsync(long doctorId, DateOnly? date)
    {
        var query = _db.Registrations.Where(r => r.DoctorId == doctorId);
        if (date.HasValue)
        {
            var from = date.Value.ToDateTime(TimeOnly.MinValue);
            var to = date.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(r => r.RegisterTime >= from && r.RegisterTime <= to);
        }
        return await query.OrderByDescending(r => r.RegisterTime).ToListAsync();
    }

    public async Task<int> GetNextQueueNumberAsync(long scheduleId, string slotName)
    {
        var max = await _db.Registrations
            .Where(r => r.ScheduleId == scheduleId && r.SlotName == slotName)
            .MaxAsync(r => (int?)r.QueueNumber) ?? 0;
        return max + 1;
    }

    public async Task AddAsync(Registration registration)
    {
        await _db.Registrations.AddAsync(registration);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Registration registration)
    {
        _db.Registrations.Update(registration);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Registrations.FindAsync(id);
        if (entity is not null)
        {
            _db.Registrations.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
