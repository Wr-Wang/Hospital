using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly List<Department> _departments = new();

    public Task<Department?> GetByIdAsync(long id)
    {
        var department = _departments.FirstOrDefault(d => d.Id == id);
        return Task.FromResult(department);
    }

    public Task<List<Department>> GetAllAsync()
    {
        return Task.FromResult(_departments.ToList());
    }

    public Task<List<Department>> GetByCampusIdAsync(long campusId)
    {
        var departments = _departments.Where(d => d.CampusId == campusId).ToList();
        return Task.FromResult(departments);
    }

    public Task<List<Department>> GetByParentIdAsync(long? parentId)
    {
        var departments = _departments.Where(d => d.ParentId == parentId).ToList();
        return Task.FromResult(departments);
    }

    public Task<List<Department>> GetTreeByCampusIdAsync(long campusId)
    {
        var departments = _departments.Where(d => d.CampusId == campusId).ToList();
        return Task.FromResult(departments);
    }

    public Task AddAsync(Department department)
    {
        department.GetType().GetProperty("Id")?.SetValue(department, _departments.Count + 1);
        _departments.Add(department);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Department department)
    {
        var index = _departments.FindIndex(d => d.Id == department.Id);
        if (index >= 0)
            _departments[index] = department;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long id)
    {
        var department = _departments.FirstOrDefault(d => d.Id == id);
        if (department is not null)
            _departments.Remove(department);
        return Task.CompletedTask;
    }
}
