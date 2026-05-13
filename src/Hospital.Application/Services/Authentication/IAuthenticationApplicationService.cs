using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IAuthenticationApplicationService
{
    Task<AuthenticationResult> LoginAsync(string username, string password);
    Task LogoutAsync();
}