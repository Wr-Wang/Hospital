using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(long id);
    Task<List<Department>> GetAllAsync();
    Task<List<Department>> GetByCampusIdAsync(long campusId);
    Task<List<Department>> GetByParentIdAsync(long? parentId);
    Task<List<Department>> GetTreeByCampusIdAsync(long campusId);
    Task AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(long id);
}
