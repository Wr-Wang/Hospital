using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientApplicationService _patientService;

    public PatientController(IPatientApplicationService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null)
        {
            return NotFound();
        }

        return Ok(patient);
    }

    [HttpGet("by-patient-no/{patientNo}")]
    public async Task<IActionResult> GetByPatientNo(string patientNo)
    {
        var patient = await _patientService.GetByPatientNoAsync(patientNo);
        if (patient is null)
        {
            return NotFound();
        }

        return Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
    {
        var createDto = new CreatePatientDto(
            request.PatientNo,
            request.Name,
            request.Gender,
            request.BirthDate,
            request.Phone,
            request.AllergiesText,
            request.IdCard);

        var id = await _patientService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}

public record CreatePatientRequest(
    string PatientNo,
    string Name,
    string? Gender,
    string? BirthDate,
    string? Phone,
    string? AllergiesText,
    string? IdCard
);