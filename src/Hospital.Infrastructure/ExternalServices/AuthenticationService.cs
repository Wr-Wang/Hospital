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

            // HTTP 200: 登录成功，API 返回 {displayName, campusName}
            return new AuthenticationResult(true, null,
                new UserInfo(1, response.DisplayName ?? string.Empty, response.CampusName ?? string.Empty));
        }
        catch (HttpRequestException)
        {
            // HTTP 400: 登录失败，API 返回 {message: "..."}
            // PostAsync 内的 EnsureSuccessStatusCode 已抛出，具体错误信息在 ex.Data 中
            return new AuthenticationResult(false, "用户名或密码错误", null);
        }
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }
}