using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>科室管理</summary>
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

    /// <summary>获取全部科室列表（平面）</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    /// <summary>获取指定院区的科室树形结构</summary>
    [HttpGet("tree/{campusId:long}")]
    public async Task<IActionResult> GetTree(long campusId)
    {
        var tree = await _departmentService.GetTreeByCampusIdAsync(campusId);
        return Ok(tree);
    }

    /// <summary>根据 ID 获取科室详情</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        if (department is null)
            return NotFound();

        return Ok(department);
    }

    /// <summary>新建科室</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var dto = new CreateDepartmentDto(request.Code, request.Name, request.CampusId, request.Type, request.ParentId);
        var id = await _departmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>更新科室信息</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDepartmentRequest request)
    {
        var dto = new UpdateDepartmentDto(request.Name, request.Type, request.ParentId);
        await _departmentService.UpdateAsync(id, dto);
        return NoContent();
    }

    /// <summary>启用科室</summary>
    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        await _departmentService.ActivateAsync(id);
        return NoContent();
    }

    /// <summary>停用科室</summary>
    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        await _departmentService.DeactivateAsync(id);
        return NoContent();
    }
}

public class CreateDepartmentRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long CampusId { get; set; }
    public string Type { get; set; } = string.Empty;
    public long? ParentId { get; set; }
}

public class UpdateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public long? ParentId { get; set; }
}
