using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfStaffRepository : IStaffRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfStaffRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<Staff?> GetByIdAsync(long id)
        => await _db.Staffs.FindAsync(id);

    public async Task<List<Staff>> GetAllAsync()
        => await _db.Staffs.ToListAsync();

    public async Task<List<Staff>> GetByCampusIdAsync(long campusId)
        => await _db.Staffs.Where(s => s.CampusId == campusId).ToListAsync();

    public async Task<List<Staff>> GetByDeptIdAsync(long deptId)
        => await _db.Staffs.Where(s => s.DeptId == deptId).ToListAsync();

    public async Task<List<Staff>> GetByLicenseExpiryAsync(DateTime expiryDate)
        => await _db.Staffs.Where(s => s.LicenseExpiry <= expiryDate).ToListAsync();

    public async Task AddAsync(Staff staff)
    {
        await _db.Staffs.AddAsync(staff);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Staff staff)
    {
        _db.Staffs.Update(staff);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.Staffs.FindAsync(id);
        if (entity is not null)
        {
            _db.Staffs.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
