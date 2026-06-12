using Hospital.Application.Repositories;
using Hospital.Domain.Aggregates.Patient;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfPatientRepository : IPatientRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfPatientRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Patient?> GetByIdAsync(long id)
        => await _db.Patients
            .Include(p => p.Identifiers)
            .Include(p => p.Consents)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Patient?> GetByPatientNoAsync(string patientNo)
        => await _db.Patients
            .Include(p => p.Identifiers)
            .Include(p => p.Consents)
            .FirstOrDefaultAsync(p => p.PatientNo == patientNo);

    public async Task<Patient?> GetByIdCardAsync(string idCard)
        => await _db.Patients
            .FirstOrDefaultAsync(p => p.IdCard != null && p.IdCard.Number == idCard);

    public async Task<List<Patient>> GetSuspectDuplicatesAsync(string name, string? phone)
    {
        var query = _db.Patients.Where(p => p.Name.Contains(name));
        if (phone is not null)
            query = query.Where(p => p.Phone != null && p.Phone.Value == phone);
        return await query.Take(10).ToListAsync();
    }

    public async Task<(List<Patient> Items, int TotalCount)> SearchAsync(string? keyword, int page, int size)
    {
        var query = _db.Patients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(p =>
                p.Name.Contains(keyword) ||
                p.PatientNo.Contains(keyword) ||
                (p.IdCard != null && p.IdCard.Number.Contains(keyword)) ||
                (p.Phone != null && p.Phone.Value.Contains(keyword)));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Patient patient)
    {
        await _db.Patients.AddAsync(patient);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Patient patient)
    {
        _db.Patients.Update(patient);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Patients.FindAsync(id);
        if (entity is not null)
        {
            _db.Patients.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
