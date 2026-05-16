using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IStaffApplicationService
{
    Task<StaffDto?> GetByIdAsync(long id);
    Task<List<StaffDto>> GetAllAsync();
    Task<List<StaffDto>> GetByCampusIdAsync(long campusId);
    Task<List<StaffDto>> GetByDeptIdAsync(long deptId);
    Task<long> CreateAsync(CreateStaffDto dto);
    Task UpdateAsync(long id, UpdateStaffDto dto);
    Task UpdateLicenseAsync(long id, UpdateStaffLicenseDto dto);
    Task ActivateAsync(long id);
    Task DeactivateAsync(long id);
}
