using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public class AuthenticationApplicationService : IAuthenticationApplicationService
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationApplicationService(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<AuthenticationResult> LoginAsync(string username, string password)
    {
        var request = new AuthenticationRequest(username, password);
        return await _authenticationService.AuthenticateAsync(request);
    }

    public Task LogoutAsync()
    {
        return _authenticationService.LogoutAsync();
    }
}