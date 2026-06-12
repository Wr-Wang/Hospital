namespace Hospital.Application.DTOs;

// ===== 院区（Campus） =====

public sealed record CampusDto(
    long Id,
    string Code,
    string Name,
    string? Address,
    string? Phone,
    bool IsActive);

public sealed record CreateCampusDto(
    string Code,
    string Name,
    string? Address,
    string? Phone);

public sealed record UpdateCampusDto(
    string Name,
    string? Address,
    string? Phone);

// ===== 科室（Department） =====

public sealed record DepartmentDto(
    long Id,
    string Code,
    string Name,
    long? ParentId,
    long CampusId,
    string Type,
    bool IsActive,
    List<DepartmentDto> Children);

public sealed record CreateDepartmentDto(
    string Code,
    string Name,
    long CampusId,
    string Type,
    long? ParentId);

public sealed record UpdateDepartmentDto(
    string Name,
    string Type,
    long? ParentId);

// ===== 人员（Staff） =====

public sealed record StaffDto(
    long Id,
    string Code,
    string Name,
    string Gender,
    string? Phone,
    long CampusId,
    long DeptId,
    string LicenseType,
    string LicenseNo,
    DateTime? LicenseExpiry,
    bool IsActive,
    bool IsLicenseExpired);

public sealed record CreateStaffDto(
    string Code,
    string Name,
    string Gender,
    string? Phone,
    long CampusId,
    long DeptId,
    string LicenseType,
    string LicenseNo,
    DateTime? LicenseExpiry);

public sealed record UpdateStaffDto(
    string Name,
    string Gender,
    string? Phone,
    long DeptId);

public sealed record UpdateStaffLicenseDto(
    string LicenseType,
    string LicenseNo,
    DateTime? LicenseExpiry);
