using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfDispenseRepository : IDispenseRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfDispenseRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Dispensing?> GetByIdAsync(long id)
        => await _db.Dispensings.Include(d => d.Items).FirstOrDefaultAsync(d => d.Id == id);

    public async Task<List<Dispensing>> GetByPrescriptionIdAsync(long prescriptionId)
        => await _db.Dispensings.Include(d => d.Items)
            .Where(d => d.PrescriptionId == prescriptionId)
            .ToListAsync();

    public async Task AddAsync(Dispensing dispensing)
    {
        await _db.Dispensings.AddAsync(dispensing);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Dispensing dispensing)
    {
        _db.Dispensings.Update(dispensing);
        await _db.SaveChangesAsync();
    }
}
