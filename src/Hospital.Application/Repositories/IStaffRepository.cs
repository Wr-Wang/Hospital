using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IStaffRepository
{
    Task<Staff?> GetByIdAsync(long id);
    Task<List<Staff>> GetAllAsync();
    Task<List<Staff>> GetByCampusIdAsync(long campusId);
    Task<List<Staff>> GetByDeptIdAsync(long deptId);
    Task<List<Staff>> GetByLicenseExpiryAsync(DateTime expiryDate);
    Task AddAsync(Staff staff);
    Task UpdateAsync(Staff staff);
    Task DeleteAsync(long id);
}
