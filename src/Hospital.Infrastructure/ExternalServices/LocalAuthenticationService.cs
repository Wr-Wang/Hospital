using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

public sealed class LocalAuthenticationService : IAuthenticationService
{
    private readonly LocalUserStore _userStore;
    private readonly JwtTokenService _tokenService;

    public LocalAuthenticationService(LocalUserStore userStore, JwtTokenService tokenService)
    {
        _userStore = userStore;
        _tokenService = tokenService;
    }

    public Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request)
    {
        var user = _userStore.FindByLoginName(request.Username);
        if (user is null || !string.Equals(user.Password, request.Password, StringComparison.Ordinal))
        {
            return Task.FromResult(new AuthenticationResult(false, "用户名或密码错误", null));
        }

        var userInfo = new UserInfo(user.Id, user.DisplayName, user.CampusName, user.Roles, user.Permissions);
        var token = _tokenService.GenerateToken(userInfo, user.Permissions);

        return Task.FromResult(new AuthenticationResult(true, null, userInfo, token));
    }

    public Task LogoutAsync() => Task.CompletedTask;
}
