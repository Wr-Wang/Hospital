using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CashierController : ControllerBase
{
    private readonly ICashierApplicationService _cashierService;

    public CashierController(ICashierApplicationService cashierService)
    {
        _cashierService = cashierService;
    }

    [HttpGet("pending-items/{patientId:long}")]
    public async Task<IActionResult> GetPendingItems(long patientId)
    {
        var items = await _cashierService.GetPendingItemsAsync(patientId);
        return Ok(items);
    }

    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromBody] PayRequestDto dto)
    {
        await _cashierService.PayAsync(dto);
        return Ok(new { message = "缴费成功" });
    }
}
