using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>排班管理</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleApplicationService _scheduleService;

    public ScheduleController(IScheduleApplicationService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    /// <summary>根据 ID 获取排班</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var dto = await _scheduleService.GetByIdAsync(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    /// <summary>根据医生获取排班</summary>
    [HttpGet("by-doctor/{doctorId:long}")]
    public async Task<IActionResult> GetByDoctor(long doctorId)
    {
        var list = await _scheduleService.GetByDoctorAsync(doctorId);
        return Ok(list);
    }

    /// <summary>根据科室获取排班</summary>
    [HttpGet("by-dept/{deptId:long}")]
    public async Task<IActionResult> GetByDept(long deptId, [FromQuery] string? date)
    {
        var list = await _scheduleService.GetByDeptAsync(deptId, date);
        return Ok(list);
    }

    /// <summary>获取可用排班</summary>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] long deptId,
        [FromQuery] long? doctorId,
        [FromQuery] string date)
    {
        var list = await _scheduleService.GetAvailableAsync(deptId, doctorId, date);
        return Ok(list);
    }

    /// <summary>新建排班</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
    {
        var slots = request.Slots.ConvertAll(s => new CreateScheduleSlotDto(
            s.SlotName, s.StartTime, s.EndTime, s.TotalQuota));

        var dto = new CreateScheduleDto(
            request.DoctorId, request.DeptId, request.CampusId,
            request.ScheduleDate, slots);

        var id = await _scheduleService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>发布排班</summary>
    [HttpPatch("{id:long}/publish")]
    public async Task<IActionResult> Publish(long id)
    {
        await _scheduleService.PublishAsync(id);
        return NoContent();
    }

    /// <summary>停用排班</summary>
    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        await _scheduleService.DeactivateAsync(id);
        return NoContent();
    }

    /// <summary>更新排班时段配额</summary>
    [HttpPut("{id:long}/slot-quota")]
    public async Task<IActionResult> UpdateSlotQuota(long id, [FromBody] UpdateScheduleSlotQuotaRequest request)
    {
        var dto = new UpdateScheduleSlotDto(request.SlotName, request.TotalQuota);
        await _scheduleService.UpdateSlotQuotaAsync(id, dto);
        return NoContent();
    }
}

// ===== Request DTOs =====

public class CreateScheduleRequest
{
    public long DoctorId { get; set; }
    public long DeptId { get; set; }
    public long CampusId { get; set; }
    public string ScheduleDate { get; set; } = string.Empty;
    public List<CreateScheduleSlotRequest> Slots { get; set; } = new();
}

public class CreateScheduleSlotRequest
{
    public string SlotName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int TotalQuota { get; set; }
}

public class UpdateScheduleSlotQuotaRequest
{
    public string SlotName { get; set; } = string.Empty;
    public int TotalQuota { get; set; }
}
