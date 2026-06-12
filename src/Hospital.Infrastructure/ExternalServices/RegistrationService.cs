using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>挂号模块 HTTP 服务实现，调用后端 RegistrationController 接口</summary>
public sealed class RegistrationService : IRegistrationApplicationService
{
    private readonly IApiClient _api;

    public RegistrationService(IApiClient api) => _api = api;

    public async Task<RegistrationDto?> GetByIdAsync(long id)
        => await _api.GetAsyncOrDefault<RegistrationDto>(ApiRoutes.Registration.ById(id));

    public async Task<List<RegistrationDto>> GetByPatientAsync(long patientId)
        => await _api.GetAsync<List<RegistrationDto>>(ApiRoutes.Registration.ByPatient(patientId));

    public async Task<List<RegistrationDto>> GetByDoctorAsync(long doctorId, string? date)
        => await _api.GetAsync<List<RegistrationDto>>(ApiRoutes.Registration.ByDoctor(doctorId, date));

    public async Task<long> RegisterAsync(CreateRegistrationDto request)
    {
        var result = await _api.PostAsync<IdResponse>(ApiRoutes.Registration.Base, request);
        return result.Id;
    }

    public async Task VoidAsync(long id)
        => await _api.PatchAsync(ApiRoutes.Registration.Void(id));

    private sealed record IdResponse(long Id);
}
