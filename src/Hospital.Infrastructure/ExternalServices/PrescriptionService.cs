using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>处方模块 HTTP 服务实现，调用后端 PrescriptionController 接口</summary>
public sealed class PrescriptionService : IPrescriptionApplicationService
{
    private readonly IApiClient _api;

    public PrescriptionService(IApiClient api) => _api = api;

    public async Task<List<PrescriptionDto>> GetByEncounterAsync(long encounterId)
        => await _api.GetAsync<List<PrescriptionDto>>(ApiRoutes.Prescription.ByEncounter(encounterId));

    public async Task<long> CreateAsync(long encounterId, long doctorId, CreatePrescriptionDto dto)
    {
        var result = await _api.PostAsync<IdResponse>(ApiRoutes.Prescription.Base, dto);
        return result.Id;
    }

    public async Task VoidAsync(long id)
        => await _api.PatchAsync(ApiRoutes.Prescription.Void(id));

    private sealed record IdResponse(long Id);
}
