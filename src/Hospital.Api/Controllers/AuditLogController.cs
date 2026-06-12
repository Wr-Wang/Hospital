using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.Repositories;

namespace Hospital.Api.Controllers;

/// <summary>操作审计日志</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogController(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    /// <summary>获取全部审计日志</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _auditLogRepository.GetAllAsync();
        return Ok(logs);
    }

    /// <summary>根据 ID 获取审计日志</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var log = await _auditLogRepository.GetByIdAsync(id);
        if (log is null)
            return NotFound();

        return Ok(log);
    }

    /// <summary>根据用户获取审计日志</summary>
    [HttpGet("by-user/{userId:long}")]
    public async Task<IActionResult> GetByUser(long userId)
    {
        var logs = await _auditLogRepository.GetByUserIdAsync(userId);
        return Ok(logs);
    }

    /// <summary>根据实体获取审计日志</summary>
    [HttpGet("by-entity")]
    public async Task<IActionResult> GetByEntity([FromQuery] string entityType, [FromQuery] long entityId)
    {
        var logs = await _auditLogRepository.GetByEntityAsync(entityType, entityId);
        return Ok(logs);
    }

    /// <summary>根据日期范围获取审计日志</summary>
    [HttpGet("by-date")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var logs = await _auditLogRepository.GetByDateRangeAsync(from, to);
        return Ok(logs);
    }
}
