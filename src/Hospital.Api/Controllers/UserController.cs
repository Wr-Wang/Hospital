using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>用户管理</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRoleApplicationService _userRoleService;

    public UserController(IUserRoleApplicationService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    /// <summary>获取当前登录用户信息</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdStr is null || !long.TryParse(userIdStr, out var userId))
            return Unauthorized(new { message = "无法识别当前用户" });

        var user = await _userRoleService.GetUserByIdAsync(userId);
        if (user is null)
            return NotFound(new { message = "用户不存在" });

        return Ok(user);
    }

    /// <summary>获取全部用户</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _userRoleService.GetAllUsersAsync();
        return Ok(list);
    }

    /// <summary>新建用户</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var id = await _userRoleService.CreateUserAsync(dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    /// <summary>更新用户</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUserDto dto)
    {
        await _userRoleService.UpdateUserAsync(id, dto);
        return NoContent();
    }
}
