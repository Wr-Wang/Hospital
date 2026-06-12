using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request);
    Task LogoutAsync();
}