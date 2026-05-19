using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfDrugInventoryRepository : IDrugInventoryRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfDrugInventoryRepository(Data.HospitalDbContext db) => _db = db;

    public async Task<DrugInventory?> GetByIdAsync(long id)
        => await _db.DrugInventories.FindAsync(id);

    public async Task<List<DrugInventory>> GetByDrugCodeAsync(string drugCode)
        => await _db.DrugInventories.Where(d => d.DrugCode == drugCode).ToListAsync();

    public async Task<List<DrugInventory>> GetAllAvailableAsync()
        => await _db.DrugInventories
            .Where(d => d.AvailableQuantity > 0 && !d.IsExpired)
            .ToListAsync();

    public async Task AddAsync(DrugInventory inventory)
    {
        await _db.DrugInventories.AddAsync(inventory);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(DrugInventory inventory)
    {
        _db.DrugInventories.Update(inventory);
        await _db.SaveChangesAsync();
    }
}
