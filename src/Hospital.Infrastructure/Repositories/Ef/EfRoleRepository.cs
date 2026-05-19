using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfRoleRepository : IRoleRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfRoleRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Role?> GetByIdAsync(long id)
        => await _db.Roles.FindAsync(id);

    public async Task<Role?> GetByNameAsync(string name)
        => await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);

    public async Task<List<Role>> GetAllAsync()
        => await _db.Roles.ToListAsync();

    public async Task AddAsync(Role role)
    {
        await _db.Roles.AddAsync(role);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Role role)
    {
        _db.Roles.Update(role);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Roles.FindAsync(id);
        if (entity is not null)
        {
            _db.Roles.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
