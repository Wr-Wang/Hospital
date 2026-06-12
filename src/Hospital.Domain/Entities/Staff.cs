using Hospital.Domain.Enums;
using Hospital.Domain.ValueObjects;

namespace Hospital.Domain.Entities;

public class Staff : Entity
{
    /// <summary>人员编码（工号）</summary>
    public string Code { get; private set; }
    /// <summary>姓名</summary>
    public string Name { get; private set; }
    /// <summary>性别</summary>
    public Gender Gender { get; private set; }
    /// <summary>联系电话</summary>
    public string? Phone { get; private set; }
    /// <summary>所属院区</summary>
    public long CampusId { get; private set; }
    /// <summary>所属科室</summary>
    public long DeptId { get; private set; }
    /// <summary>执业资质类型（执业医师/执业护士/药师/医技）</summary>
    public LicenseType LicenseType { get; private set; }
    /// <summary>执业资质编号</summary>
    public LicenseNumber LicenseNo { get; private set; }
    /// <summary>资质有效期，过期后禁止高风险操作</summary>
    public DateTime? LicenseExpiry { get; private set; }
    /// <summary>启用状态</summary>
    public bool IsActive { get; private set; } = true;

    // EF Core 无参构造
    private Staff()
    {
        Code = default!;
        Name = default!;
        LicenseNo = default!;
    }

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
