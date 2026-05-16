using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

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

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var dto = await _registrationService.GetByIdAsync(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("by-patient/{patientId:long}")]
    public async Task<IActionResult> GetByPatient(long patientId)
    {
        var list = await _registrationService.GetByPatientAsync(patientId);
        return Ok(list);
    }

    [HttpGet("by-doctor/{doctorId:long}")]
    public async Task<IActionResult> GetByDoctor(long doctorId, [FromQuery] string? date)
    {
        var list = await _registrationService.GetByDoctorAsync(doctorId, date);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateRegistrationRequest request)
    {
        var dto = new CreateRegistrationDto(
            request.PatientId, request.ScheduleId, request.DoctorId,
            request.DeptId, request.CampusId, request.SlotName);

        var id = await _registrationService.RegisterAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPatch("{id:long}/void")]
    public async Task<IActionResult> Void(long id)
    {
        await _registrationService.VoidAsync(id);
        return NoContent();
    }
}

// ===== Request Records =====

public record CreateRegistrationRequest(
    long PatientId,
    long ScheduleId,
    long DoctorId,
    long DeptId,
    long CampusId,
    string SlotName);
