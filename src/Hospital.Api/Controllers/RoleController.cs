using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>角色管理</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IUserRoleApplicationService _userRoleService;

    public RoleController(IUserRoleApplicationService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    /// <summary>获取全部角色</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _userRoleService.GetAllRolesAsync();
        return Ok(list);
    }

    /// <summary>新建角色</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        var id = await _userRoleService.CreateRoleAsync(dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    /// <summary>更新角色</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateRoleDto dto)
    {
        await _userRoleService.UpdateRoleAsync(id, dto);
        return NoContent();
    }

    /// <summary>删除角色</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _userRoleService.DeleteRoleAsync(id);
        return NoContent();
    }
}
