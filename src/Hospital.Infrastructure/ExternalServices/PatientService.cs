using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Hospital.Application.Constants;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Infrastructure.ExternalServices;

/// <summary>
/// 患者模块的 HTTP 服务实现，包装 IApiClient 调用后端 PatientController 接口。
/// 遵循与 AuthenticationService 相同的模式：WPF → Application Service → ExternalServices → API。
/// </summary>
public sealed class PatientService : IPatientApplicationService
{
    private readonly IApiClient _api;

    public PatientService(IApiClient api)
    {
        _api = api;
    }

    /// <summary>按 ID 查询患者，不存在时返回 null</summary>
    public async Task<PatientDto?> GetByIdAsync(long id)
        => await _api.GetAsyncOrDefault<PatientDto>(ApiRoutes.Patient.ById(id));

    /// <summary>按病历号查询患者</summary>
    public async Task<PatientDto?> GetByPatientNoAsync(string patientNo)
        => await _api.GetAsyncOrDefault<PatientDto>(ApiRoutes.Patient.ByPatientNo(patientNo));

    /// <summary>按身份证号查重，不存在时返回 null</summary>
    public async Task<PatientDto?> GetByIdCardAsync(string idCard)
        => await _api.GetAsyncOrDefault<PatientDto>(ApiRoutes.Patient.ByIdCard(idCard));

    /// <summary>按姓名+手机号模糊匹配，用于建档时提示疑似重复</summary>
    public async Task<List<PatientDto>> GetSuspectDuplicatesAsync(string name, string? phone)
        => await _api.PostAsync<List<PatientDto>>(ApiRoutes.Patient.SuspectDuplicates, new { name, phone });

    /// <summary>模糊搜索（姓名/身份证/手机号/病历号），支持分页</summary>
    public async Task<PatientSearchResultDto> SearchAsync(string? keyword, int page, int size)
        => await _api.GetAsync<PatientSearchResultDto>(ApiRoutes.Patient.Search(keyword, page, size));

    /// <summary>获取患者 360 视图（含就诊历史）</summary>
    public async Task<PatientProfileDto?> GetProfileAsync(long id)
        => await _api.GetAsyncOrDefault<PatientProfileDto>(ApiRoutes.Patient.Profile(id));

    /// <summary>创建患者建档，返回新患者 ID</summary>
    public async Task<long> CreateAsync(CreatePatientDto request)
    {
        var result = await _api.PostAsync<IdResponse>(ApiRoutes.Patient.Base, request);
        return result.Id;
    }

    /// <summary>API 返回的 ID 响应体</summary>
    private sealed record IdResponse(long Id);
}
