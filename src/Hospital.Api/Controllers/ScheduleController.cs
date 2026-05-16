using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

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

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var dto = await _scheduleService.GetByIdAsync(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("by-doctor/{doctorId:long}")]
    public async Task<IActionResult> GetByDoctor(long doctorId)
    {
        var list = await _scheduleService.GetByDoctorAsync(doctorId);
        return Ok(list);
    }

    [HttpGet("by-dept/{deptId:long}")]
    public async Task<IActionResult> GetByDept(long deptId, [FromQuery] string? date)
    {
        var list = await _scheduleService.GetByDeptAsync(deptId, date);
        return Ok(list);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] long deptId,
        [FromQuery] long? doctorId,
        [FromQuery] string date)
    {
        var list = await _scheduleService.GetAvailableAsync(deptId, doctorId, date);
        return Ok(list);
    }

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

    [HttpPatch("{id:long}/publish")]
    public async Task<IActionResult> Publish(long id)
    {
        await _scheduleService.PublishAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        await _scheduleService.DeactivateAsync(id);
        return NoContent();
    }

    [HttpPut("{id:long}/slot-quota")]
    public async Task<IActionResult> UpdateSlotQuota(long id, [FromBody] UpdateScheduleSlotQuotaRequest request)
    {
        var dto = new UpdateScheduleSlotDto(request.SlotName, request.TotalQuota);
        await _scheduleService.UpdateSlotQuotaAsync(id, dto);
        return NoContent();
    }
}

// ===== Request Records =====

public record CreateScheduleRequest(
    long DoctorId,
    long DeptId,
    long CampusId,
    string ScheduleDate,
    List<CreateScheduleSlotRequest> Slots);

public record CreateScheduleSlotRequest(
    string SlotName,
    string StartTime,
    string EndTime,
    int TotalQuota);

public record UpdateScheduleSlotQuotaRequest(string SlotName, int TotalQuota);
