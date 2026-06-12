using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface ICampusRepository
{
    Task<Campus?> GetByIdAsync(long id);
    Task<Campus?> GetByCodeAsync(string code);
    Task<List<Campus>> GetAllAsync();
    Task<List<Campus>> GetActiveAsync();
    Task AddAsync(Campus campus);
    Task UpdateAsync(Campus campus);
    Task DeleteAsync(long id);
}
