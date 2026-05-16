using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>收费模块 HTTP 服务实现，调用后端 CashierController 接口</summary>
public sealed class CashierService : ICashierApplicationService
{
    private readonly IApiClient _api;

    public CashierService(IApiClient api) => _api = api;

    public async Task<List<ChargeItemDto>> GetPendingItemsAsync(long patientId)
        => await _api.GetAsync<List<ChargeItemDto>>(ApiRoutes.Cashier.PendingItems(patientId));

    public async Task PayAsync(PayRequestDto dto)
        => await _api.PostAsync<object>(ApiRoutes.Cashier.Pay, dto);
}
