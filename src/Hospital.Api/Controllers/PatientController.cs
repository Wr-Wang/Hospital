using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>患者管理</summary>
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

    /// <summary>根据 ID 获取患者</summary>    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient is null)
            return NotFound();

        return Ok(patient);
    }

    /// <summary>根据病历号获取患者</summary>    [HttpGet("by-patient-no/{patientNo}")]
    public async Task<IActionResult> GetByPatientNo(string patientNo)
    {
        var patient = await _patientService.GetByPatientNoAsync(patientNo);
        if (patient is null)
            return NotFound();

        return Ok(patient);
    }

    /// <summary>根据身份证号获取患者</summary>    [HttpGet("by-idcard/{idCard}")]
    public async Task<IActionResult> GetByIdCard(string idCard)
    {
        var patient = await _patientService.GetByIdCardAsync(idCard);
        if (patient is null)
            return NotFound();

        return Ok(patient);
    }

    /// <summary>查询疑似重复患者</summary>    [HttpPost("suspect-duplicates")]
    public async Task<IActionResult> GetSuspectDuplicates([FromBody] SuspectDuplicateRequest request)
    {
        var patients = await _patientService.GetSuspectDuplicatesAsync(request.Name, request.Phone);
        return Ok(patients);
    }

    /// <summary>搜索患者（分页）</summary>    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20)
    {
        var result = await _patientService.SearchAsync(keyword, page, size);
        return Ok(result);
    }

    /// <summary>获取患者详细档案</summary>    [HttpGet("{id}/profile")]
    public async Task<IActionResult> GetProfile(long id)
    {
        var profile = await _patientService.GetProfileAsync(id);
        if (profile is null)
            return NotFound();

        return Ok(profile);
    }

    /// <summary>新建患者</summary>
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

public class CreatePatientRequest
{
    public string PatientNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? BirthDate { get; set; }
    public string? Phone { get; set; }
    public string? AllergiesText { get; set; }
    public string? IdCard { get; set; }
}

public class SuspectDuplicateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
