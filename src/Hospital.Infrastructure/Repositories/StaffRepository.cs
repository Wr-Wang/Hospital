using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly List<Staff> _staffList = new();

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
