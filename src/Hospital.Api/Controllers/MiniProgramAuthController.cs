using Hospital.Application.DTOs;
using Hospital.Application.Services.WeChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Api.Controllers;

/// <summary>小程序认证（微信登录）</summary>
[ApiController]
[Route("api/miniprogram/auth")]
public class MiniProgramAuthController : ControllerBase
{
    private readonly IWeChatAuthService _weChatAuthService;

    public MiniProgramAuthController(IWeChatAuthService weChatAuthService)
    {
        _weChatAuthService = weChatAuthService;
    }

    /// <summary>code → openid → 已绑定则直接 JWT，否则返回临时 token</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] WeChatLoginRequest request)
    {
        var result = await _weChatAuthService.LoginAsync(request);
        return Ok(result);
    }

    /// <summary>创建新患者并绑定微信</summary>
    [HttpPost("create-patient")]
    public async Task<IActionResult> CreatePatient([FromBody] WeChatCreatePatientRequest request)
    {
        var result = await _weChatAuthService.CreatePatientAsync(request.TempToken, request.Name, request.Phone);
        return Ok(result);
    }

    /// <summary>刷新 access_token</summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _weChatAuthService.RefreshTokenAsync(request);
        return Ok(result);
    }

    /// <summary>获取当前登录患者的资料</summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentPatient()
    {
        var patientId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _weChatAuthService.GetCurrentPatientAsync(patientId);
        return Ok(result);
    }

    /// <summary>退出登录（撤销 refresh_token）</summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var patientId = long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        await _weChatAuthService.LogoutAsync(patientId, request.RefreshToken);
        return Ok();
    }
}

public sealed record WeChatCreatePatientRequest(string TempToken, string Name, string? Phone = null);

public sealed record LogoutRequest(string RefreshToken);
