using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>诊断模块 HTTP 服务实现，调用后端 DiagnosisController 接口</summary>
public sealed class DiagnosisService : IDiagnosisApplicationService
{
    private readonly IApiClient _api;

    public DiagnosisService(IApiClient api) => _api = api;

    public async Task<List<DiagnosisDto>> GetByEncounterAsync(long encounterId)
        => await _api.GetAsync<List<DiagnosisDto>>(ApiRoutes.Diagnosis.ByEncounter(encounterId));

    public async Task<long> AddAsync(long encounterId, CreateDiagnosisDto dto)
    {
        var result = await _api.PostAsync<IdResponse>(ApiRoutes.Diagnosis.Base, dto);
        return result.Id;
    }

    public async Task RemoveAsync(long id)
        => await _api.DeleteAsync(ApiRoutes.Diagnosis.ById(id));

    private sealed record IdResponse(long Id);
}
