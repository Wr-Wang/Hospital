using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IDepartmentApplicationService
{
    Task<DepartmentDto?> GetByIdAsync(long id);
    Task<List<DepartmentDto>> GetAllAsync();
    Task<List<DepartmentDto>> GetTreeByCampusIdAsync(long campusId);
    Task<long> CreateAsync(CreateDepartmentDto dto);
    Task UpdateAsync(long id, UpdateDepartmentDto dto);
    Task ActivateAsync(long id);
    Task DeactivateAsync(long id);
}
