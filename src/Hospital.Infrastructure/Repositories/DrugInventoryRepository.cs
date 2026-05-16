using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class DrugInventoryRepository : IDrugInventoryRepository
{
    private readonly List<DrugInventory> _inventories = new();

    public Task<DrugInventory?> GetByIdAsync(long id)
        => Task.FromResult(_inventories.FirstOrDefault(i => i.Id == id));

    public Task<List<DrugInventory>> GetByDrugCodeAsync(string drugCode)
        => Task.FromResult(_inventories.Where(i => i.DrugCode == drugCode).ToList());

    public Task<List<DrugInventory>> GetAllAvailableAsync()
        => Task.FromResult(_inventories.Where(i => i.AvailableQuantity > 0 && !i.IsExpired).ToList());

    public Task AddAsync(DrugInventory inventory)
    {
        inventory.GetType().GetProperty("Id")?.SetValue(inventory, _inventories.Count + 1);
        _inventories.Add(inventory);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(DrugInventory inventory)
    {
        var index = _inventories.FindIndex(i => i.Id == inventory.Id);
        if (index >= 0)
            _inventories[index] = inventory;
        return Task.CompletedTask;
    }
}
