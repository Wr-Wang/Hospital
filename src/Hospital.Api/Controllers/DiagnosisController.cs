using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DiagnosisController : ControllerBase
{
    private readonly IDiagnosisApplicationService _diagnosisService;

    public DiagnosisController(IDiagnosisApplicationService diagnosisService)
    {
        _diagnosisService = diagnosisService;
    }

    [HttpGet("by-encounter/{encounterId:long}")]
    public async Task<IActionResult> GetByEncounter(long encounterId)
    {
        var list = await _diagnosisService.GetByEncounterAsync(encounterId);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Add(long encounterId, [FromBody] CreateDiagnosisDto dto)
    {
        var id = await _diagnosisService.AddAsync(encounterId, dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remove(long id)
    {
        await _diagnosisService.RemoveAsync(id);
        return NoContent();
    }
}
