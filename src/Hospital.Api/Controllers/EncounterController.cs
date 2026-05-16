using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EncounterController : ControllerBase
{
    private readonly IEncounterApplicationService _encounterService;

    public EncounterController(IEncounterApplicationService encounterService)
    {
        _encounterService = encounterService;
    }

    [HttpGet("queue")]
    public async Task<IActionResult> GetQueue([FromQuery] long doctorId, [FromQuery] string date)
    {
        var list = await _encounterService.GetQueueAsync(doctorId, date);
        return Ok(list);
    }

    [HttpPatch("{id:long}/start")]
    public async Task<IActionResult> StartConsultation(long id)
    {
        await _encounterService.StartConsultationAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:long}/complete")]
    public async Task<IActionResult> CompleteConsultation(long id)
    {
        await _encounterService.CompleteConsultationAsync(id);
        return NoContent();
    }
}
