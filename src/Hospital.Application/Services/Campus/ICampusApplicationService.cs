using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface ICampusApplicationService
{
    Task<CampusDto?> GetByIdAsync(long id);
    Task<List<CampusDto>> GetAllAsync();
    Task<List<CampusDto>> GetActiveAsync();
    Task<long> CreateAsync(CreateCampusDto request);
    Task UpdateAsync(long id, UpdateCampusDto request);
    Task ActivateAsync(long id);
    Task DeactivateAsync(long id);
}
