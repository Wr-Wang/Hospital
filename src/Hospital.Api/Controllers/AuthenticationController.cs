using Microsoft.AspNetCore.Mvc;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationApplicationService _authenticationService;

    public AuthenticationController(IAuthenticationApplicationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authenticationService.LoginAsync(request.Username, request.Password);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(new
        {
            displayName = result.UserInfo?.DisplayName,
            campusName = result.UserInfo?.CampusName
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authenticationService.LogoutAsync();
        return Ok();
    }
}

public record LoginRequest(string Username, string Password);