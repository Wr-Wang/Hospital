using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>病历管理</summary>
[Authorize]
[ApiController]
[Route("api/medical-record")]
public class MedicalRecordController : ControllerBase
{
    private readonly IMedicalRecordApplicationService _recordService;

    public MedicalRecordController(IMedicalRecordApplicationService recordService)
    {
        _recordService = recordService;
    }

    /// <summary>根据就诊 ID 获取病历</summary>
    [HttpGet("by-encounter/{encounterId:long}")]
    public async Task<IActionResult> GetByEncounter(long encounterId)
    {
        var dto = await _recordService.GetByEncounterAsync(encounterId);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    /// <summary>保存病历</summary>
    [HttpPost("{encounterId:long}")]
    public async Task<IActionResult> Save(long encounterId, [FromBody] SaveMedicalRecordDto dto)
    {
        await _recordService.SaveAsync(encounterId, dto);
        return NoContent();
    }
}
