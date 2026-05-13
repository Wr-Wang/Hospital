using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

public sealed class LocalAuthenticationService : IAuthenticationService
{
    public Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request)
    {
        if (string.Equals(request.Username, "admin", StringComparison.OrdinalIgnoreCase) &&
            request.Password == "password")
        {
            var userInfo = new UserInfo(1, "系统管理员", "总院区");
            return Task.FromResult(new AuthenticationResult(true, null, userInfo));
        }

        return Task.FromResult(new AuthenticationResult(false, "用户名或密码错误", null));
    }

    public Task LogoutAsync() => Task.CompletedTask;
}
