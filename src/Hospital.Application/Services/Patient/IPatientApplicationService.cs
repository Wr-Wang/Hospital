using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IPatientApplicationService
{
    Task<PatientDto?> GetByIdAsync(long id);
    Task<PatientDto?> GetByPatientNoAsync(string patientNo);
    Task<PatientDto?> GetByIdCardAsync(string idCard);
    Task<List<PatientDto>> GetSuspectDuplicatesAsync(string name, string? phone);
    Task<PatientSearchResultDto> SearchAsync(string? keyword, int page, int size);
    Task<PatientProfileDto?> GetProfileAsync(long id);
    Task<long> CreateAsync(CreatePatientDto request);
}
