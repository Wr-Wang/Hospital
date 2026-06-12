using System.Collections.Generic;

namespace Hospital.Application.DTOs;

public sealed record UserDto(
    long Id,
    string LoginName,
    string DisplayName,
    string CampusName,
    bool IsActive,
    List<string> Roles);

public sealed record CreateUserDto(
    string LoginName,
    string Password,
    string DisplayName,
    string CampusName,
    List<string> Roles);

public sealed record UpdateUserDto(
    string? Password,
    string? DisplayName,
    bool? IsActive,
    List<string>? Roles);

public sealed record RoleDto(
    long Id,
    string Name,
    string Description,
    List<string> Permissions);

public sealed record CreateRoleDto(
    string Name,
    string Description,
    List<string> Permissions);

public sealed record UpdateRoleDto(
    string? Description,
    List<string>? Permissions);
