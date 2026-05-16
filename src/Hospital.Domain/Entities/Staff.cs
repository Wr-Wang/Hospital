using Hospital.Domain.Enums;
using Hospital.Domain.ValueObjects;

namespace Hospital.Domain.Entities;

public class Staff : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public Gender Gender { get; private set; }
    public string? Phone { get; private set; }
    public long CampusId { get; private set; }
    public long DeptId { get; private set; }
    public LicenseType LicenseType { get; private set; }
    public LicenseNumber LicenseNo { get; private set; }
    public DateTime? LicenseExpiry { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Staff()
    {
        Code = default!;
        Name = default!;
        LicenseNo = default!;
    } // For EF Core

    public Staff(string code, string name, Gender gender, string? phone,
        long campusId, long deptId, LicenseType licenseType, LicenseNumber licenseNo, DateTime? licenseExpiry)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Gender = gender;
        Phone = phone;
        CampusId = campusId;
        DeptId = deptId;
        LicenseType = licenseType;
        LicenseNo = licenseNo ?? throw new ArgumentNullException(nameof(licenseNo));
        LicenseExpiry = licenseExpiry;
    }

    public void UpdateInfo(string name, Gender gender, string? phone, long deptId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Gender = gender;
        Phone = phone;
        DeptId = deptId;
    }

    public void UpdateLicense(LicenseType licenseType, LicenseNumber licenseNo, DateTime? licenseExpiry)
    {
        LicenseType = licenseType;
        LicenseNo = licenseNo ?? throw new ArgumentNullException(nameof(licenseNo));
        LicenseExpiry = licenseExpiry;
    }

    public bool IsLicenseExpired() => LicenseExpiry.HasValue && LicenseExpiry.Value < DateTime.Today;

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
