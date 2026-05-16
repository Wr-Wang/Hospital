namespace Hospital.Application.DTOs;

public record AuthenticationRequest(string Username, string Password);

public record AuthenticationResult(bool IsSuccess, string? ErrorMessage, UserInfo? UserInfo, string? Token = null);

public record UserInfo(long Id, string DisplayName, string CampusName, string[]? Roles = null, string[]? Permissions = null);