using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>发药模块 HTTP 服务实现，调用后端 DispenseController 接口</summary>
public sealed class DispenseService : IDispenseApplicationService
{
    private readonly IApiClient _api;

    public DispenseService(IApiClient api) => _api = api;

    public async Task<List<PrescriptionDto>> GetPaidPrescriptionsAsync(long patientId)
        => await _api.GetAsync<List<PrescriptionDto>>(ApiRoutes.Dispense.PaidPrescriptions(patientId));

    public async Task DispenseAsync(long id)
        => await _api.PostAsync<object>(ApiRoutes.Dispense.DispenseItem(id), new { });

    public async Task ReturnAsync(long id)
        => await _api.PostAsync<object>(ApiRoutes.Dispense.ReturnItem(id), new { });
}
