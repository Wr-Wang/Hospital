using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _userRoleService.GetAllRolesAsync();
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        var id = await _userRoleService.CreateRoleAsync(dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateRoleDto dto)
    {
        await _userRoleService.UpdateRoleAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _userRoleService.DeleteRoleAsync(id);
        return NoContent();
    }
}
