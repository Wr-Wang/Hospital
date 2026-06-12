using Hospital.Application.Repositories;
using Hospital.Domain;
using Hospital.Domain.Entities;
using Hospital.Domain.Enums;
using Hospital.Domain.ValueObjects;

namespace Hospital.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly List<Staff> _staffList = new();

    public StaffRepository()
    {
        var seeds = new Staff[]
        {
            new("DOC001", "张三", Gender.Male, "13800138001", 1, 1, LicenseType.执业医师, new LicenseNumber("110000000000001"), new DateTime(2028, 12, 31)),
            new("DOC002", "李四", Gender.Female, "13800138002", 1, 2, LicenseType.执业医师, new LicenseNumber("110000000000002"), new DateTime(2028, 12, 31)),
            new("DOC003", "王医生", Gender.Male, "13800138003", 1, 4, LicenseType.执业医师, new LicenseNumber("110000000000003"), new DateTime(2027, 6, 30)),
            new("NUR001", "赵护士", Gender.Female, "13800138004", 1, 1, LicenseType.执业护士, new LicenseNumber("210000000000001"), new DateTime(2027, 6, 30)),
            new("NUR002", "孙护士", Gender.Female, "13800138005", 1, 3, LicenseType.执业护士, new LicenseNumber("210000000000002"), new DateTime(2027, 6, 30)),
            new("PHA001", "周药师", Gender.Male, "13800138006", 1, 8, LicenseType.药师, new LicenseNumber("310000000000001"), new DateTime(2028, 12, 31)),
            new("TEC001", "吴技师", Gender.Female, "13800138007", 1, 6, LicenseType.医技, new LicenseNumber("410000000000001"), new DateTime(2026, 12, 31)),
            new("TEC002", "郑技师", Gender.Male, "13800138008", 1, 7, LicenseType.医技, new LicenseNumber("410000000000002"), new DateTime(2026, 12, 31)),
        };
        for (int i = 0; i < seeds.Length; i++)
        {
            typeof(Entity).GetProperty("Id")?.SetValue(seeds[i], i + 1);
            _staffList.Add(seeds[i]);
        }
    }

    public Task<Staff?> GetByIdAsync(long id)
    {
        var staff = _staffList.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(staff);
    }

    public Task<List<Staff>> GetAllAsync()
    {
        return Task.FromResult(_staffList.ToList());
    }

    public Task<List<Staff>> GetByCampusIdAsync(long campusId)
    {
        var staffList = _staffList.Where(s => s.CampusId == campusId).ToList();
        return Task.FromResult(staffList);
    }

    public Task<List<Staff>> GetByDeptIdAsync(long deptId)
    {
        var staffList = _staffList.Where(s => s.DeptId == deptId).ToList();
        return Task.FromResult(staffList);
    }

    public Task<List<Staff>> GetByLicenseExpiryAsync(DateTime expiryDate)
    {
        var staffList = _staffList
            .Where(s => s.LicenseExpiry.HasValue && s.LicenseExpiry.Value <= expiryDate)
            .ToList();
        return Task.FromResult(staffList);
    }

    public Task AddAsync(Staff staff)
    {
        staff.GetType().GetProperty("Id")?.SetValue(staff, _staffList.Count + 1);
        _staffList.Add(staff);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Staff staff)
    {
        var index = _staffList.FindIndex(s => s.Id == staff.Id);
        if (index >= 0)
            _staffList[index] = staff;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long id)
    {
        var staff = _staffList.FirstOrDefault(s => s.Id == id);
        if (staff is not null)
            _staffList.Remove(staff);
        return Task.CompletedTask;
    }
}
