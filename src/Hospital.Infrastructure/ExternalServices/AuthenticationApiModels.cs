namespace Hospital.Infrastructure.ExternalServices;

public sealed record ApiAuthenticationRequest(string UserName, string Password);

public sealed record AuthenticationResponse(long? Id, string? Token, string? DisplayName, string? CampusName, string[]? Roles, string[]? Permissions, long? CampusId);
