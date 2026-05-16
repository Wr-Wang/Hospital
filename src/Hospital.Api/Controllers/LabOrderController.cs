using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/lab-order")]
public class LabOrderController : ControllerBase
{
    private readonly ILabOrderApplicationService _labOrderService;

    public LabOrderController(ILabOrderApplicationService labOrderService)
    {
        _labOrderService = labOrderService;
    }

    [HttpGet("by-encounter/{encounterId:long}")]
    public async Task<IActionResult> GetByEncounter(long encounterId)
    {
        var list = await _labOrderService.GetByEncounterAsync(encounterId);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create(long encounterId, [FromBody] CreateLabOrderDto dto)
    {
        var id = await _labOrderService.CreateAsync(encounterId, dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    [HttpPatch("{id:long}/cancel")]
    public async Task<IActionResult> Cancel(long id)
    {
        await _labOrderService.CancelAsync(id);
        return NoContent();
    }
}
