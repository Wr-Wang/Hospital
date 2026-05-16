using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>病历模块 HTTP 服务实现，调用后端 MedicalRecordController 接口</summary>
public sealed class MedicalRecordService : IMedicalRecordApplicationService
{
    private readonly IApiClient _api;

    public MedicalRecordService(IApiClient api) => _api = api;

    public async Task<MedicalRecordDto?> GetByEncounterAsync(long encounterId)
        => await _api.GetAsyncOrDefault<MedicalRecordDto>(ApiRoutes.MedicalRecord.ByEncounter(encounterId));

    public async Task SaveAsync(long encounterId, SaveMedicalRecordDto dto)
        => await _api.PostAsync<object>(ApiRoutes.MedicalRecord.Save(encounterId), dto);
}
