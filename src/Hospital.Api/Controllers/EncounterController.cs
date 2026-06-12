using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>接诊管理（门诊队列）</summary>
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

    /// <summary>获取门诊队列</summary>
    [HttpGet("queue")]
    public async Task<IActionResult> GetQueue([FromQuery] long doctorId, [FromQuery] string date)
    {
        var list = await _encounterService.GetQueueAsync(doctorId, date);
        return Ok(list);
    }

    /// <summary>开始接诊</summary>
    [HttpPatch("{id:long}/start")]
    public async Task<IActionResult> StartConsultation(long id)
    {
        await _encounterService.StartConsultationAsync(id);
        return NoContent();
    }

    /// <summary>完成接诊</summary>
    [HttpPatch("{id:long}/complete")]
    public async Task<IActionResult> CompleteConsultation(long id)
    {
        await _encounterService.CompleteConsultationAsync(id);
        return NoContent();
    }
}
