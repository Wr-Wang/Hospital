using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Hospital.Domain.Enums;
using Hospital.Domain.ValueObjects;

namespace Hospital.Application.Services;

public sealed class DepartmentApplicationService : IDepartmentApplicationService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentApplicationService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<DepartmentDto?> GetByIdAsync(long id)
    {
        var department = await _repository.GetByIdAsync(id);
        return MapToDto(department);
    }

    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        var departments = await _repository.GetAllAsync();
        return departments.Select(MapToDto).ToList();
    }

    public async Task<List<DepartmentDto>> GetTreeByCampusIdAsync(long campusId)
    {
        var departments = await _repository.GetTreeByCampusIdAsync(campusId);
        return BuildTree(departments, null);
    }

    public async Task<long> CreateAsync(CreateDepartmentDto dto)
    {
        var code = new DepartmentCode(dto.Code);
        var type = Enum.Parse<DepartmentType>(dto.Type);
        var department = new Department(code, dto.Name, dto.CampusId, type, dto.ParentId);
        await _repository.AddAsync(department);
        return department.Id;
    }

    public async Task UpdateAsync(long id, UpdateDepartmentDto dto)
    {
        var department = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"科室不存在 (Id={id})");

        var type = Enum.Parse<DepartmentType>(dto.Type);
        department.UpdateInfo(dto.Name, type, dto.ParentId);
        await _repository.UpdateAsync(department);
    }

    public async Task ActivateAsync(long id)
    {
        var department = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"科室不存在 (Id={id})");

        department.Activate();
        await _repository.UpdateAsync(department);
    }

    public async Task DeactivateAsync(long id)
    {
        var department = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"科室不存在 (Id={id})");

        // 检查是否有子科室，有则禁止停用
        var children = await _repository.GetByParentIdAsync(id);
        if (children.Count != 0)
            throw new InvalidOperationException("该科室下有子科室，无法停用");

        department.Deactivate();
        await _repository.UpdateAsync(department);
    }

    private static DepartmentDto MapToDto(Department? department)
    {
        if (department is null) return null!;

        return new DepartmentDto(
            department.Id,
            department.Code,
            department.Name,
            department.ParentId,
            department.CampusId,
            department.Type.ToString(),
            department.IsActive,
            department.Children.Select(MapToDto).ToList());
    }

    private static List<DepartmentDto> BuildTree(List<Department> departments, long? parentId)
    {
        return departments
            .Where(d => d.ParentId == parentId)
            .Select(d => new DepartmentDto(
                d.Id,
                d.Code,
                d.Name,
                d.ParentId,
                d.CampusId,
                d.Type.ToString(),
                d.IsActive,
                BuildTree(departments, d.Id)))
            .ToList();
    }
}
