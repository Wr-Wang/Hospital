using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IUserRoleApplicationService
{
    // Users
    Task<List<UserDto>> GetAllUsersAsync();
    Task<long> CreateUserAsync(CreateUserDto dto);
    Task UpdateUserAsync(long id, UpdateUserDto dto);

    // Roles
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<long> CreateRoleAsync(CreateRoleDto dto);
    Task UpdateRoleAsync(long id, UpdateRoleDto dto);
    Task DeleteRoleAsync(long id);
}
