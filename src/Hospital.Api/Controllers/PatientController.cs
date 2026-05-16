using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[Authorize]
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
            return NotFound();

        return Ok(patient);
    }

    [HttpGet("by-patient-no/{patientNo}")]
    public async Task<IActionResult> GetByPatientNo(string patientNo)
    {
        var patient = await _patientService.GetByPatientNoAsync(patientNo);
        if (patient is null)
            return NotFound();

        return Ok(patient);
    }

    [HttpGet("by-idcard/{idCard}")]
    public async Task<IActionResult> GetByIdCard(string idCard)
    {
        var patient = await _patientService.GetByIdCardAsync(idCard);
        if (patient is null)
            return NotFound();

        return Ok(patient);
    }

    [HttpPost("suspect-duplicates")]
    public async Task<IActionResult> GetSuspectDuplicates([FromBody] SuspectDuplicateRequest request)
    {
        var patients = await _patientService.GetSuspectDuplicatesAsync(request.Name, request.Phone);
        return Ok(patients);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20)
    {
        var result = await _patientService.SearchAsync(keyword, page, size);
        return Ok(result);
    }

    [HttpGet("{id}/profile")]
    public async Task<IActionResult> GetProfile(long id)
    {
        var profile = await _patientService.GetProfileAsync(id);
        if (profile is null)
            return NotFound();

        return Ok(profile);
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
    string? IdCard);

public record SuspectDuplicateRequest(string Name, string? Phone);
