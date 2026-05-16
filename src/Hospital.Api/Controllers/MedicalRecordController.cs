using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

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

    [HttpGet("by-encounter/{encounterId:long}")]
    public async Task<IActionResult> GetByEncounter(long encounterId)
    {
        var dto = await _recordService.GetByEncounterAsync(encounterId);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpPost("{encounterId:long}")]
    public async Task<IActionResult> Save(long encounterId, [FromBody] SaveMedicalRecordDto dto)
    {
        await _recordService.SaveAsync(encounterId, dto);
        return NoContent();
    }
}
