namespace Hospital.Infrastructure.ExternalServices;

public sealed record ApiAuthenticationRequest(string UserName, string Password);

public sealed record AuthenticationResponse(bool Success, string? DisplayName, string? CampusName, string? ErrorMessage);
