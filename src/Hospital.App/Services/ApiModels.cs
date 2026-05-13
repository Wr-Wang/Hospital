namespace Hospital.App.Services;

public sealed record AuthenticationRequest(string UserName, string Password);
public sealed record AuthenticationResponse(bool Success, string? DisplayName, string? CampusName, string? ErrorMessage);
