using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IPatientApplicationService
{
    Task<PatientDto?> GetByIdAsync(long id);
    Task<PatientDto?> GetByPatientNoAsync(string patientNo);
    Task<long> CreateAsync(CreatePatientDto request);
}
