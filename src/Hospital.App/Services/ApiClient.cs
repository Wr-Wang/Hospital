using System;
using System.Threading.Tasks;

namespace Hospital.App.Services;

public sealed class ApiClient : IApiClient
{
    public async Task<TResponse> GetAsync<TResponse>(string route)
    {
        await Task.Delay(100);
        throw new NotImplementedException($"GET {route} is not implemented.");
    }

    public async Task<TResponse> PostAsync<TResponse>(string route, object payload)
    {
        await Task.Delay(200);

        if (route == "auth/login" && payload is AuthenticationRequest request)
        {
            var success = request.UserName == "admin" && request.Password == "password";
            var response = new AuthenticationResponse(
                Success: success,
                DisplayName: success ? "管理员" : null,
                CampusName: success ? "总部院区" : null,
                ErrorMessage: success ? null : "用户名或密码错误。"
            );

            return (TResponse)(object)response!;
        }

        throw new NotImplementedException($"POST {route} is not implemented.");
    }
}
