using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new()
    {
        CreateUser(1, "admin", "admin123", "系统管理员", "总院区", "ADMIN", "REGISTRATION", "PHARMACY", "CASHIER"),
        CreateUser(2, "doctor", "doctor123", "张医生", "总院区", "DOCTOR"),
        CreateUser(3, "reg", "reg123", "李挂号", "总院区", "REGISTRATION"),
        CreateUser(4, "pharm", "pharm123", "王药房", "总院区", "PHARMACY"),
        CreateUser(5, "cash", "cash123", "赵收费", "总院区", "CASHIER"),
    };

    private static User CreateUser(long id, string login, string pwd, string name, string campus, params string[] roles)
    {
        var user = new User(login, pwd, name, campus);
        user.GetType().GetProperty("Id")?.SetValue(user, id);
        user.SetRoles(roles);
        return user;
    }

    public Task<User?> GetByIdAsync(long id)
        => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    public Task<User?> GetByLoginNameAsync(string loginName)
        => Task.FromResult(_users.FirstOrDefault(u =>
            string.Equals(u.LoginName, loginName, System.StringComparison.OrdinalIgnoreCase)));

    public Task<List<User>> GetAllAsync()
        => Task.FromResult(_users.ToList());

    public Task AddAsync(User user)
    {
        user.GetType().GetProperty("Id")?.SetValue(user, _users.Count + 1);
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user)
    {
        return Task.CompletedTask;
    }
}
