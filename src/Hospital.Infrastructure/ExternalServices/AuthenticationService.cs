using System.Net.Http;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApiClient _apiClient;

    public AuthenticationService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request)
    {
        var apiRequest = new ApiAuthenticationRequest(request.Username, request.Password);
        try
        {
            var response = await _apiClient.PostAsync<AuthenticationResponse>("Authentication/login", apiRequest);

            // HTTP 200: 登录成功，API 返回 {token, displayName, campusName, roles}
            return new AuthenticationResult(true, null,
                new UserInfo(1, response.DisplayName ?? string.Empty, response.CampusName ?? string.Empty, response.Roles),
                response.Token);
        }
        catch (HttpRequestException)
        {
            // HTTP 401: 登录失败，API 返回 401
            return new AuthenticationResult(false, "用户名或密码错误", null);
        }
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }
}