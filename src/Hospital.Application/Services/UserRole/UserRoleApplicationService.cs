using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Application.Services;

public sealed class UserRoleApplicationService : IUserRoleApplicationService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserRoleApplicationService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => new UserDto(
            u.Id, u.LoginName, u.DisplayName, u.CampusName, u.IsActive, u.Roles.ToList())).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return null;
        return new UserDto(user.Id, user.LoginName, user.DisplayName, user.CampusName, user.IsActive, user.Roles.ToList());
    }

    public async Task<long> CreateUserAsync(CreateUserDto dto)
    {
        var existing = await _userRepository.GetByLoginNameAsync(dto.LoginName);
        if (existing is not null)
            throw new InvalidOperationException($"登录名 '{dto.LoginName}' 已存在");

        var user = new User(dto.LoginName, dto.Password, dto.DisplayName, dto.CampusName);
        user.SetRoles(dto.Roles);
        await _userRepository.AddAsync(user);
        return user.Id;
    }

    public async Task UpdateUserAsync(long id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"用户不存在 (Id={id})");

        if (dto.Password is not null)
            user.ChangePassword(dto.Password);
        if (dto.DisplayName is not null)
            user.GetType().GetProperty("DisplayName")?.SetValue(user, dto.DisplayName);
        if (dto.IsActive.HasValue)
            user.SetActive(dto.IsActive.Value);
        if (dto.Roles is not null)
            user.SetRoles(dto.Roles);

        await _userRepository.UpdateAsync(user);
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(r => new RoleDto(
            r.Id, r.Name, r.Description, r.Permissions.ToList())).ToList();
    }

    public async Task<long> CreateRoleAsync(CreateRoleDto dto)
    {
        var existing = await _roleRepository.GetByNameAsync(dto.Name);
        if (existing is not null)
            throw new InvalidOperationException($"角色 '{dto.Name}' 已存在");

        var role = new Role(dto.Name, dto.Description);
        role.SetPermissions(dto.Permissions);
        await _roleRepository.AddAsync(role);
        return role.Id;
    }

    public async Task UpdateRoleAsync(long id, UpdateRoleDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"角色不存在 (Id={id})");

        if (dto.Description is not null)
            role.GetType().GetProperty("Description")?.SetValue(role, dto.Description);
        if (dto.Permissions is not null)
            role.SetPermissions(dto.Permissions);

        await _roleRepository.UpdateAsync(role);
    }

    public async Task DeleteRoleAsync(long id)
    {
        await _roleRepository.DeleteAsync(id);
    }
}
