using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfDepartmentRepository : IDepartmentRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfDepartmentRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Department?> GetByIdAsync(long id)
        => await _db.Departments.FindAsync(id);

    public async Task<List<Department>> GetAllAsync()
        => await _db.Departments.ToListAsync();

    public async Task<List<Department>> GetByCampusIdAsync(long campusId)
        => await _db.Departments.Where(d => d.CampusId == campusId).ToListAsync();

    public async Task<List<Department>> GetByParentIdAsync(long? parentId)
        => await _db.Departments.Where(d => d.ParentId == parentId).ToListAsync();

    public async Task<List<Department>> GetTreeByCampusIdAsync(long campusId)
        => await _db.Departments.Where(d => d.CampusId == campusId).ToListAsync();

    public async Task AddAsync(Department department)
    {
        await _db.Departments.AddAsync(department);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Department department)
    {
        _db.Departments.Update(department);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Departments.FindAsync(id);
        if (entity is not null)
        {
            _db.Departments.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
