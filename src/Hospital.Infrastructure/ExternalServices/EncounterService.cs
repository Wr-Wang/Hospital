using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>就诊模块 HTTP 服务实现，调用后端 EncounterController 接口</summary>
public sealed class EncounterService : IEncounterApplicationService
{
    private readonly IApiClient _api;

    public EncounterService(IApiClient api) => _api = api;

    public async Task<List<EncounterQueueItemDto>> GetQueueAsync(long doctorId, string date)
        => await _api.GetAsync<List<EncounterQueueItemDto>>(ApiRoutes.Encounter.Queue(doctorId, date));

    public async Task StartConsultationAsync(long id)
        => await _api.PatchAsync(ApiRoutes.Encounter.StartConsultation(id));

    public async Task CompleteConsultationAsync(long id)
        => await _api.PatchAsync(ApiRoutes.Encounter.CompleteConsultation(id));
}
