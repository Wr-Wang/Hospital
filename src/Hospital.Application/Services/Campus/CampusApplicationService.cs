using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Hospital.Domain.ValueObjects;

namespace Hospital.Application.Services;

public sealed class CampusApplicationService : ICampusApplicationService
{
    private readonly ICampusRepository _repository;

    public CampusApplicationService(ICampusRepository repository)
    {
        _repository = repository;
    }

    public async Task<CampusDto?> GetByIdAsync(long id)
    {
        var campus = await _repository.GetByIdAsync(id);
        return MapToDto(campus);
    }

    public async Task<List<CampusDto>> GetAllAsync()
    {
        var campuses = await _repository.GetAllAsync();
        return campuses.Select(MapToDto).ToList();
    }

    public async Task<List<CampusDto>> GetActiveAsync()
    {
        var campuses = await _repository.GetActiveAsync();
        return campuses.Select(MapToDto).ToList();
    }

    public async Task<long> CreateAsync(CreateCampusDto request)
    {
        var code = new CampusCode(request.Code);
        var campus = new Campus(code, request.Name, request.Address, request.Phone);
        await _repository.AddAsync(campus);
        return campus.Id;
    }

    public async Task UpdateAsync(long id, UpdateCampusDto request)
    {
        var campus = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"院区不存在 (Id={id})");

        campus.UpdateInfo(request.Name, request.Address, request.Phone);
        await _repository.UpdateAsync(campus);
    }

    public async Task ActivateAsync(long id)
    {
        var campus = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"院区不存在 (Id={id})");

        campus.Activate();
        await _repository.UpdateAsync(campus);
    }

    public async Task DeactivateAsync(long id)
    {
        var campus = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"院区不存在 (Id={id})");

        campus.Deactivate();
        await _repository.UpdateAsync(campus);
    }

    private static CampusDto MapToDto(Campus? campus)
    {
        if (campus is null) return null!;

        return new CampusDto(
            campus.Id,
            campus.Code,
            campus.Name,
            campus.Address,
            campus.Phone,
            campus.IsActive);
    }
}
