using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>检验单管理</summary>
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

    /// <summary>根据就诊 ID 获取检验单</summary>
    [HttpGet("by-encounter/{encounterId:long}")]
    public async Task<IActionResult> GetByEncounter(long encounterId)
    {
        var list = await _labOrderService.GetByEncounterAsync(encounterId);
        return Ok(list);
    }

    /// <summary>创建检验单</summary>
    [HttpPost]
    public async Task<IActionResult> Create(long encounterId, [FromBody] CreateLabOrderDto dto)
    {
        var id = await _labOrderService.CreateAsync(encounterId, dto);
        return CreatedAtAction(null, new { id }, new { id });
    }

    /// <summary>取消检验单</summary>
    [HttpPatch("{id:long}/cancel")]
    public async Task<IActionResult> Cancel(long id)
    {
        await _labOrderService.CancelAsync(id);
        return NoContent();
    }
}
