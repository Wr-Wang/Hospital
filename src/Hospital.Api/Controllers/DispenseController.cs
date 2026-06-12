using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

/// <summary>发药管理</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DispenseController : ControllerBase
{
    private readonly IDispenseApplicationService _dispenseService;

    public DispenseController(IDispenseApplicationService dispenseService)
    {
        _dispenseService = dispenseService;
    }

    /// <summary>获取已缴费处方</summary>
    [HttpGet("paid-prescriptions/{patientId:long}")]
    public async Task<IActionResult> GetPaidPrescriptions(long patientId)
    {
        var list = await _dispenseService.GetPaidPrescriptionsAsync(patientId);
        return Ok(list);
    }

    /// <summary>发药</summary>
    [HttpPost("{id:long}/dispense")]
    public async Task<IActionResult> Dispense(long id)
    {
        await _dispenseService.DispenseAsync(id);
        return Ok(new { message = "发药成功" });
    }

    /// <summary>退药</summary>
    [HttpPost("{id:long}/return")]
    public async Task<IActionResult> Return(long id)
    {
        await _dispenseService.ReturnAsync(id);
        return Ok(new { message = "退药成功" });
    }
}
