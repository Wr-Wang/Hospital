using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

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

    [HttpGet("paid-prescriptions/{patientId:long}")]
    public async Task<IActionResult> GetPaidPrescriptions(long patientId)
    {
        var list = await _dispenseService.GetPaidPrescriptionsAsync(patientId);
        return Ok(list);
    }

    [HttpPost("{id:long}/dispense")]
    public async Task<IActionResult> Dispense(long id)
    {
        await _dispenseService.DispenseAsync(id);
        return Ok(new { message = "发药成功" });
    }

    [HttpPost("{id:long}/return")]
    public async Task<IActionResult> Return(long id)
    {
        await _dispenseService.ReturnAsync(id);
        return Ok(new { message = "退药成功" });
    }
}
