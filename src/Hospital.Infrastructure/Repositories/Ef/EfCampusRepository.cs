using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfCampusRepository : ICampusRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfCampusRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Campus?> GetByIdAsync(long id)
        => await _db.Campuses.FindAsync(id);

    public async Task<Campus?> GetByCodeAsync(string code)
        => await _db.Campuses.FirstOrDefaultAsync(c => c.Code != null && c.Code.Value == code);

    public async Task<List<Campus>> GetAllAsync()
        => await _db.Campuses.ToListAsync();

    public async Task<List<Campus>> GetActiveAsync()
        => await _db.Campuses.Where(c => c.IsActive).ToListAsync();

    public async Task AddAsync(Campus campus)
    {
        await _db.Campuses.AddAsync(campus);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Campus campus)
    {
        _db.Campuses.Update(campus);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Campuses.FindAsync(id);
        if (entity is not null)
        {
            _db.Campuses.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
