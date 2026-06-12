using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>检验申请 HTTP 服务实现，调用后端 LabOrderController 接口</summary>
public sealed class LabOrderService : ILabOrderApplicationService
{
    private readonly IApiClient _api;

    public LabOrderService(IApiClient api) => _api = api;

    public async Task<List<LabOrderDto>> GetByEncounterAsync(long encounterId)
        => await _api.GetAsync<List<LabOrderDto>>(ApiRoutes.LabOrder.ByEncounter(encounterId));

    public async Task<long> CreateAsync(long encounterId, CreateLabOrderDto dto)
    {
        var result = await _api.PostAsync<IdResponse>(ApiRoutes.LabOrder.Base, dto);
        return result.Id;
    }

    public async Task CancelAsync(long id)
        => await _api.PatchAsync(ApiRoutes.LabOrder.Cancel(id));

    private sealed record IdResponse(long Id);
}
