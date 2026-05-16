using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Hospital.Domain.Enums;
using Hospital.Domain.ValueObjects;

namespace Hospital.Application.Services;

public sealed class StaffApplicationService : IStaffApplicationService
{
    private readonly IStaffRepository _repository;

    public StaffApplicationService(IStaffRepository repository)
    {
        _repository = repository;
    }

    public async Task<StaffDto?> GetByIdAsync(long id)
    {
        var staff = await _repository.GetByIdAsync(id);
        return MapToDto(staff);
    }

    public async Task<List<StaffDto>> GetAllAsync()
    {
        var staffList = await _repository.GetAllAsync();
        return staffList.Select(MapToDto).ToList();
    }

    public async Task<List<StaffDto>> GetByCampusIdAsync(long campusId)
    {
        var staffList = await _repository.GetByCampusIdAsync(campusId);
        return staffList.Select(MapToDto).ToList();
    }

    public async Task<List<StaffDto>> GetByDeptIdAsync(long deptId)
    {
        var staffList = await _repository.GetByDeptIdAsync(deptId);
        return staffList.Select(MapToDto).ToList();
    }

    public async Task<long> CreateAsync(CreateStaffDto dto)
    {
        var gender = Enum.Parse<Gender>(dto.Gender);
        var licenseType = Enum.Parse<LicenseType>(dto.LicenseType);
        var licenseNo = new LicenseNumber(dto.LicenseNo);

        var staff = new Staff(dto.Code, dto.Name, gender, dto.Phone,
            dto.CampusId, dto.DeptId, licenseType, licenseNo, dto.LicenseExpiry);
        await _repository.AddAsync(staff);
        return staff.Id;
    }

    public async Task UpdateAsync(long id, UpdateStaffDto dto)
    {
        var staff = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"人员不存在 (Id={id})");

        var gender = Enum.Parse<Gender>(dto.Gender);
        staff.UpdateInfo(dto.Name, gender, dto.Phone, dto.DeptId);
        await _repository.UpdateAsync(staff);
    }

    public async Task UpdateLicenseAsync(long id, UpdateStaffLicenseDto dto)
    {
        var staff = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"人员不存在 (Id={id})");

        var licenseType = Enum.Parse<LicenseType>(dto.LicenseType);
        var licenseNo = new LicenseNumber(dto.LicenseNo);
        staff.UpdateLicense(licenseType, licenseNo, dto.LicenseExpiry);
        await _repository.UpdateAsync(staff);
    }

    public async Task ActivateAsync(long id)
    {
        var staff = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"人员不存在 (Id={id})");

        staff.Activate();
        await _repository.UpdateAsync(staff);
    }

    public async Task DeactivateAsync(long id)
    {
        var staff = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"人员不存在 (Id={id})");

        staff.Deactivate();
        await _repository.UpdateAsync(staff);
    }

    private static StaffDto MapToDto(Staff? staff)
    {
        if (staff is null) return null!;

        return new StaffDto(
            staff.Id,
            staff.Code,
            staff.Name,
            staff.Gender.ToString(),
            staff.Phone,
            staff.CampusId,
            staff.DeptId,
            staff.LicenseType.ToString(),
            staff.LicenseNo,
            staff.LicenseExpiry,
            staff.IsActive,
            staff.IsLicenseExpired());
    }
}
