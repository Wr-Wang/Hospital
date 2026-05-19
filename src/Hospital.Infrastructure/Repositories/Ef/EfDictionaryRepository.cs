using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Infrastructure.Repositories.Ef;

public sealed class EfDictionaryRepository : IDictionaryRepository
{
    private readonly Data.HospitalDbContext _db;

    public EfDictionaryRepository(Data.HospitalDbContext db) => _db = db;

    // DictionaryType methods
    public async Task<DictionaryType?> GetTypeByIdAsync(long id)
        => await _db.DictionaryTypes.FindAsync(id);

    public async Task<DictionaryType?> GetTypeByCodeAsync(string code)
        => await _db.DictionaryTypes.FirstOrDefaultAsync(t => t.Code == code);

    public async Task<List<DictionaryType>> GetAllTypesAsync()
        => await _db.DictionaryTypes.ToListAsync();

    public async Task AddTypeAsync(DictionaryType type)
    {
        await _db.DictionaryTypes.AddAsync(type);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateTypeAsync(DictionaryType type)
    {
        _db.DictionaryTypes.Update(type);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteTypeAsync(long id)
    {
        var entity = await _db.DictionaryTypes.FindAsync(id);
        if (entity is not null)
        {
            _db.DictionaryTypes.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }

    // DictionaryItem methods
    public async Task<DictionaryItem?> GetItemByIdAsync(long id)
        => await _db.DictionaryItems.FindAsync(id);

    public async Task<List<DictionaryItem>> GetItemsByTypeIdAsync(long typeId)
        => await _db.DictionaryItems
            .Where(i => i.TypeId == typeId)
            .OrderBy(i => i.SortOrder)
            .ToListAsync();

    public async Task<List<DictionaryItem>> GetItemsByTypeCodeAsync(string typeCode)
    {
        var type = await _db.DictionaryTypes.FirstOrDefaultAsync(t => t.Code == typeCode);
        if (type is null) return new List<DictionaryItem>();

        return await _db.DictionaryItems
            .Where(i => i.TypeId == type.Id)
            .OrderBy(i => i.SortOrder)
            .ToListAsync();
    }

    public async Task AddItemAsync(DictionaryItem item)
    {
        await _db.DictionaryItems.AddAsync(item);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(DictionaryItem item)
    {
        _db.DictionaryItems.Update(item);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(long id)
    {
        var entity = await _db.DictionaryItems.FindAsync(id);
        if (entity is not null)
        {
            _db.DictionaryItems.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
