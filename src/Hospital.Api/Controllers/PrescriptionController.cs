using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

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

    [HttpGet("by-encounter/{encounterId:long}")]
    public async Task<IActionResult> GetByEncounter(long encounterId)
    {
        var list = await _prescriptionService.GetByEncounterAsync(encounterId);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create(long encounterId, long doctorId, [FromBody] CreatePrescriptionDto dto)
    {
        var id = await _prescriptionService.CreateAsync(encounterId, doctorId, dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    [HttpPatch("{id:long}/void")]
    public async Task<IActionResult> Void(long id)
    {
        await _prescriptionService.VoidAsync(id);
        return NoContent();
    }
}
