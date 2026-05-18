using Hospital.Application.Repositories;
using Hospital.Domain;
using Hospital.Domain.Entities;
using Hospital.Domain.ValueObjects;

namespace Hospital.Infrastructure.Repositories;

public class CampusRepository : ICampusRepository
{
    private readonly List<Campus> _campuses = new();

    public CampusRepository()
    {
        var seeds = new Campus[]
        {
            new(new CampusCode("HQ"), "总院区", "北京市海淀区中关村大街1号", "010-12345678"),
            new(new CampusCode("East"), "东院区", "北京市朝阳区建国路88号", "010-87654321"),
            new(new CampusCode("West"), "西院区", "北京市海淀区西直门北大街32号", "010-56781234"),
        };
        for (int i = 0; i < seeds.Length; i++)
        {
            typeof(Entity).GetProperty("Id")?.SetValue(seeds[i], i + 1);
            _campuses.Add(seeds[i]);
        }
    }

    public Task<Campus?> GetByIdAsync(long id)
    {
        var campus = _campuses.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(campus);
    }

    public Task<Campus?> GetByCodeAsync(string code)
    {
        var campus = _campuses.FirstOrDefault(c => c.Code.Value == code);
        return Task.FromResult(campus);
    }

    public Task<List<Campus>> GetAllAsync()
    {
        return Task.FromResult(_campuses.ToList());
    }

    public Task<List<Campus>> GetActiveAsync()
    {
        var active = _campuses.Where(c => c.IsActive).ToList();
        return Task.FromResult(active);
    }

    public Task AddAsync(Campus campus)
    {
        campus.GetType().GetProperty("Id")?.SetValue(campus, _campuses.Count + 1);
        _campuses.Add(campus);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Campus campus)
    {
        var index = _campuses.FindIndex(c => c.Id == campus.Id);
        if (index >= 0)
            _campuses[index] = campus;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long id)
    {
        var campus = _campuses.FirstOrDefault(c => c.Id == id);
        if (campus is not null)
            _campuses.Remove(campus);
        return Task.CompletedTask;
    }
}
