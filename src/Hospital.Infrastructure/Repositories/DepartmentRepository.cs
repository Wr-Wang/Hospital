using Hospital.Application.Repositories;
using Hospital.Domain;
using Hospital.Domain.Entities;
using Hospital.Domain.Enums;
using Hospital.Domain.ValueObjects;

namespace Hospital.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly List<Department> _departments = new();

    public DepartmentRepository()
    {
        var seeds = new Department[]
        {
            new(new DepartmentCode("K-NK"), "内科", 1, DepartmentType.门诊),
            new(new DepartmentCode("K-WK"), "外科", 1, DepartmentType.门诊),
            new(new DepartmentCode("K-FK"), "妇产科", 1, DepartmentType.门诊),
            new(new DepartmentCode("K-EK"), "儿科", 1, DepartmentType.门诊),
            new(new DepartmentCode("K-GK"), "骨科", 1, DepartmentType.门诊),
            new(new DepartmentCode("K-FSK"), "放射科", 1, DepartmentType.医技),
            new(new DepartmentCode("K-JYK"), "检验科", 1, DepartmentType.医技),
            new(new DepartmentCode("YF"), "药房", 1, DepartmentType.药房),
            new(new DepartmentCode("K-ZZ"), "行政办公室", 1, DepartmentType.行政),
        };
        for (int i = 0; i < seeds.Length; i++)
        {
            typeof(Entity).GetProperty("Id")?.SetValue(seeds[i], i + 1);
            _departments.Add(seeds[i]);
        }
    }

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
