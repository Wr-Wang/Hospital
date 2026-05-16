using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _userRoleService.GetAllUsersAsync();
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var id = await _userRoleService.CreateUserAsync(dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUserDto dto)
    {
        await _userRoleService.UpdateUserAsync(id, dto);
        return NoContent();
    }
}
