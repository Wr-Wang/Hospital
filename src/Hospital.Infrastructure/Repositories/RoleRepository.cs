using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly List<Role> _roles = new()
    {
        CreateRole(1, "ADMIN", "系统管理员 — 所有权限",
            Permissions.ShellUse, Permissions.SecurityManage,
            Permissions.CampusManage, Permissions.DeptManage, Permissions.StaffManage, Permissions.DictManage,
            Permissions.PatientRegister, Permissions.PatientSearch,
            Permissions.Schedule, Permissions.RegisterWork, Permissions.Encounter,
            Permissions.Dispense, Permissions.Cashier),

        CreateRole(2, "DOCTOR", "门诊医生 — 医生站 + 患者检索",
            Permissions.ShellUse,
            Permissions.PatientSearch,
            Permissions.Encounter,
            Permissions.Schedule),

        CreateRole(3, "REGISTRATION", "挂号员 — 挂号 + 排班 + 患者建档/检索",
            Permissions.ShellUse,
            Permissions.PatientRegister, Permissions.PatientSearch,
            Permissions.Schedule, Permissions.RegisterWork),

        CreateRole(4, "PHARMACY", "药房人员 — 发药工作台",
            Permissions.ShellUse,
            Permissions.Dispense),

        CreateRole(5, "CASHIER", "收费员 — 收费工作台",
            Permissions.ShellUse,
            Permissions.Cashier),
    };

    private static Role CreateRole(long id, string name, string description, params string[] permissions)
    {
        var role = new Role(name, description);
        role.GetType().GetProperty("Id")?.SetValue(role, id);
        role.SetPermissions(permissions);
        return role;
    }

    public Task<Role?> GetByIdAsync(long id)
        => Task.FromResult(_roles.FirstOrDefault(r => r.Id == id));

    public Task<Role?> GetByNameAsync(string name)
        => Task.FromResult(_roles.FirstOrDefault(r => r.Name == name));

    public Task<List<Role>> GetAllAsync()
        => Task.FromResult(_roles.ToList());

    public Task AddAsync(Role role)
    {
        role.GetType().GetProperty("Id")?.SetValue(role, _roles.Count + 1);
        _roles.Add(role);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Role role)
    {
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long id)
    {
        var role = _roles.FirstOrDefault(r => r.Id == id);
        if (role is not null)
            _roles.Remove(role);
        return Task.CompletedTask;
    }
}
