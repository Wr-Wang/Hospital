using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentApplicationService _departmentService;

    public DepartmentController(IDepartmentApplicationService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    [HttpGet("tree/{campusId:long}")]
    public async Task<IActionResult> GetTree(long campusId)
    {
        var tree = await _departmentService.GetTreeByCampusIdAsync(campusId);
        return Ok(tree);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        if (department is null)
            return NotFound();

        return Ok(department);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var dto = new CreateDepartmentDto(request.Code, request.Name, request.CampusId, request.Type, request.ParentId);
        var id = await _departmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDepartmentRequest request)
    {
        var dto = new UpdateDepartmentDto(request.Name, request.Type, request.ParentId);
        await _departmentService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        await _departmentService.ActivateAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        await _departmentService.DeactivateAsync(id);
        return NoContent();
    }
}

public record CreateDepartmentRequest(string Code, string Name, long CampusId, string Type, long? ParentId);
public record UpdateDepartmentRequest(string Name, string Type, long? ParentId);
