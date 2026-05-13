using System.Threading.Tasks;

namespace Hospital.App.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IApiClient _apiClient;

    public AuthenticationService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return new AuthenticationResult(false, ErrorMessage: "请输入用户名和密码。");
        }

        var response = await _apiClient.PostAsync<AuthenticationResponse>("auth/login", new AuthenticationRequest(userName, password));

        return new AuthenticationResult(response.Success, response.DisplayName, response.CampusName, response.ErrorMessage);
    }
}
