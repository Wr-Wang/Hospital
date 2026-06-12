using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IDrugInventoryRepository
{
    Task<DrugInventory?> GetByIdAsync(long id);
    Task<List<DrugInventory>> GetByDrugCodeAsync(string drugCode);
    Task<List<DrugInventory>> GetAllAvailableAsync();
    Task AddAsync(DrugInventory inventory);
    Task UpdateAsync(DrugInventory inventory);
}
