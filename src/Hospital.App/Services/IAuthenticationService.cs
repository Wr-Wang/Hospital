using System.Threading.Tasks;

namespace Hospital.App.Services;

public sealed record AuthenticationResult(bool Success, string? DisplayName = null, string? CampusName = null, string? ErrorMessage = null);

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string userName, string password);
}
