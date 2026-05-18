using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>人员管理</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IStaffApplicationService _staffService;

    public StaffController(IStaffApplicationService staffService)
    {
        _staffService = staffService;
    }

    /// <summary>获取全部人员列表</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var staffList = await _staffService.GetAllAsync();
        return Ok(staffList);
    }

    /// <summary>根据院区获取人员列表</summary>
    [HttpGet("by-campus/{campusId:long}")]
    public async Task<IActionResult> GetByCampus(long campusId)
    {
        var staffList = await _staffService.GetByCampusIdAsync(campusId);
        return Ok(staffList);
    }

    /// <summary>根据科室获取人员列表</summary>
    [HttpGet("by-dept/{deptId:long}")]
    public async Task<IActionResult> GetByDepartment(long deptId)
    {
        var staffList = await _staffService.GetByDeptIdAsync(deptId);
        return Ok(staffList);
    }

    /// <summary>根据 ID 获取人员详情</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var staff = await _staffService.GetByIdAsync(id);
        if (staff is null)
            return NotFound();

        return Ok(staff);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStaffRequest request)
    {
        var dto = new CreateStaffDto(
            request.Code, request.Name, request.Gender, request.Phone,
            request.CampusId, request.DeptId, request.LicenseType,
            request.LicenseNo, request.LicenseExpiry);
        var id = await _staffService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateStaffRequest request)
    {
        var dto = new UpdateStaffDto(request.Name, request.Gender, request.Phone, request.DeptId);
        await _staffService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id:long}/license")]
    public async Task<IActionResult> UpdateLicense(long id, [FromBody] UpdateStaffLicenseRequest request)
    {
        var dto = new UpdateStaffLicenseDto(request.LicenseType, request.LicenseNo, request.LicenseExpiry);
        await _staffService.UpdateLicenseAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        await _staffService.ActivateAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        await _staffService.DeactivateAsync(id);
        return NoContent();
    }
}

public class CreateStaffRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public long CampusId { get; set; }
    public long DeptId { get; set; }
    public string LicenseType { get; set; } = string.Empty;
    public string LicenseNo { get; set; } = string.Empty;
    public DateTime? LicenseExpiry { get; set; }
}

public class UpdateStaffRequest
{
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public long DeptId { get; set; }
}

public class UpdateStaffLicenseRequest
{
    public string LicenseType { get; set; } = string.Empty;
    public string LicenseNo { get; set; } = string.Empty;
    public DateTime? LicenseExpiry { get; set; }
}
