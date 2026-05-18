using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>挂号管理</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationApplicationService _registrationService;

    public RegistrationController(IRegistrationApplicationService registrationService)
    {
        _registrationService = registrationService;
    }

    /// <summary>根据 ID 获取挂号记录</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var dto = await _registrationService.GetByIdAsync(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    /// <summary>获取患者的就诊历史</summary>
    [HttpGet("by-patient/{patientId:long}")]
    public async Task<IActionResult> GetByPatient(long patientId)
    {
        var list = await _registrationService.GetByPatientAsync(patientId);
        return Ok(list);
    }

    /// <summary>获取医生的挂号列表</summary>
    [HttpGet("by-doctor/{doctorId:long}")]
    public async Task<IActionResult> GetByDoctor(long doctorId, [FromQuery] string? date)
    {
        var list = await _registrationService.GetByDoctorAsync(doctorId, date);
        return Ok(list);
    }

    /// <summary>新建挂号</summary>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateRegistrationRequest request)
    {
        var dto = new CreateRegistrationDto(
            request.PatientId, request.ScheduleId, request.DoctorId,
            request.DeptId, request.CampusId, request.SlotName);

        var id = await _registrationService.RegisterAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>作废挂号</summary>
    [HttpPatch("{id:long}/void")]
    public async Task<IActionResult> Void(long id)
    {
        await _registrationService.VoidAsync(id);
        return NoContent();
    }
}

// ===== Request DTOs =====

public class CreateRegistrationRequest
{
    public long PatientId { get; set; }
    public long ScheduleId { get; set; }
    public long DoctorId { get; set; }
    public long DeptId { get; set; }
    public long CampusId { get; set; }
    public string SlotName { get; set; } = string.Empty;
}
