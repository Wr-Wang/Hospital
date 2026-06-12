using Microsoft.AspNetCore.Mvc;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>用户认证（登录/注销）</summary>
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationApplicationService _authenticationService;

    public AuthenticationController(IAuthenticationApplicationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>用户登录，返回 JWT Token</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authenticationService.LoginAsync(request.Username, request.Password);

        if (!result.IsSuccess)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        return Ok(new
        {
            token = result.Token,
            displayName = result.UserInfo?.DisplayName,
            campusName = result.UserInfo?.CampusName,
            roles = result.UserInfo?.Roles,
            permissions = result.UserInfo?.Permissions
        });
    }

    /// <summary>用户注销</summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authenticationService.LogoutAsync();
        return Ok();
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}