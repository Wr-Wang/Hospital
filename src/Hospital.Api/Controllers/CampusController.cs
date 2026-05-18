using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CampusController : ControllerBase
{
    private readonly ICampusApplicationService _campusService;

    public CampusController(ICampusApplicationService campusService)
    {
        _campusService = campusService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var campuses = await _campusService.GetAllAsync();
        return Ok(campuses);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var campuses = await _campusService.GetActiveAsync();
        return Ok(campuses);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var campus = await _campusService.GetByIdAsync(id);
        if (campus is null)
            return NotFound();

        return Ok(campus);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCampusRequest request)
    {
        var dto = new CreateCampusDto(request.Code, request.Name, request.Address, request.Phone);
        var id = await _campusService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCampusRequest request)
    {
        var dto = new UpdateCampusDto(request.Name, request.Address, request.Phone);
        await _campusService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        await _campusService.ActivateAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        await _campusService.DeactivateAsync(id);
        return NoContent();
    }
}

public class CreateCampusRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
}

public class UpdateCampusRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
}
