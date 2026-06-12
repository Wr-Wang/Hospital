using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>诊断管理</summary>
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

    /// <summary>根据就诊 ID 获取诊断</summary>
    [HttpGet("by-encounter/{encounterId:long}")]
    public async Task<IActionResult> GetByEncounter(long encounterId)
    {
        var list = await _diagnosisService.GetByEncounterAsync(encounterId);
        return Ok(list);
    }

    /// <summary>添加诊断</summary>
    [HttpPost]
    public async Task<IActionResult> Add(long encounterId, [FromBody] CreateDiagnosisDto dto)
    {
        var id = await _diagnosisService.AddAsync(encounterId, dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    /// <summary>移除诊断</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remove(long id)
    {
        await _diagnosisService.RemoveAsync(id);
        return NoContent();
    }
}
