using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>用户角色模块 HTTP 服务实现</summary>
public sealed class UserRoleService : IUserRoleApplicationService
{
    private readonly IApiClient _api;

    public UserRoleService(IApiClient api) => _api = api;

    public async Task<List<UserDto>> GetAllUsersAsync()
        => await _api.GetAsync<List<UserDto>>(ApiRoutes.User.Base);

    public async Task<UserDto?> GetUserByIdAsync(long id)
        => await _api.GetAsync<UserDto>(ApiRoutes.User.ById(id));

    public async Task<long> CreateUserAsync(CreateUserDto dto)
    {
        var result = await _api.PostAsync<IdResponse>(ApiRoutes.User.Base, dto);
        return result.Id;
    }

    public async Task UpdateUserAsync(long id, UpdateUserDto dto)
        => await _api.PutAsync(ApiRoutes.User.ById(id), dto);

    public async Task<List<RoleDto>> GetAllRolesAsync()
        => await _api.GetAsync<List<RoleDto>>(ApiRoutes.Role.Base);

    public async Task<long> CreateRoleAsync(CreateRoleDto dto)
    {
        var result = await _api.PostAsync<IdResponse>(ApiRoutes.Role.Base, dto);
        return result.Id;
    }

    public async Task UpdateRoleAsync(long id, UpdateRoleDto dto)
        => await _api.PutAsync(ApiRoutes.Role.ById(id), dto);

    public async Task DeleteRoleAsync(long id)
        => await _api.DeleteAsync(ApiRoutes.Role.ById(id));

    private sealed record IdResponse(long Id);
}
