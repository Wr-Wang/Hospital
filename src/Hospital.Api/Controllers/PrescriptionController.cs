using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>处方管理</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PrescriptionController : ControllerBase
{
    private readonly IPrescriptionApplicationService _prescriptionService;

    public PrescriptionController(IPrescriptionApplicationService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    /// <summary>根据就诊 ID 获取处方</summary>
    [HttpGet("by-encounter/{encounterId:long}")]
    public async Task<IActionResult> GetByEncounter(long encounterId)
    {
        var list = await _prescriptionService.GetByEncounterAsync(encounterId);
        return Ok(list);
    }

    /// <summary>创建处方</summary>
    [HttpPost]
    public async Task<IActionResult> Create(long encounterId, long doctorId, [FromBody] CreatePrescriptionDto dto)
    {
        var id = await _prescriptionService.CreateAsync(encounterId, doctorId, dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    /// <summary>作废处方</summary>
    [HttpPatch("{id:long}/void")]
    public async Task<IActionResult> Void(long id)
    {
        await _prescriptionService.VoidAsync(id);
        return NoContent();
    }
}
