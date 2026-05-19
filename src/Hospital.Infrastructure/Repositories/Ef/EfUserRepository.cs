using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfUserRepository : IUserRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfUserRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(long id)
        => await _db.Users.FindAsync(id);

    public async Task<User?> GetByLoginNameAsync(string loginName)
        => await _db.Users.FirstOrDefaultAsync(u => u.LoginName == loginName);

    public async Task<List<User>> GetAllAsync()
        => await _db.Users.ToListAsync();

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Users.FindAsync(id);
        if (entity is not null)
        {
            _db.Users.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
