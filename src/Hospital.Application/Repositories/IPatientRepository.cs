using Hospital.Domain.Aggregates.Patient;

namespace Hospital.Application.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(long id);
    Task<Patient?> GetByPatientNoAsync(string patientNo);
    Task<Patient?> GetByIdCardAsync(string idCard);
    Task<List<Patient>> GetSuspectDuplicatesAsync(string name, string? phone);
    Task<(List<Patient> Items, int TotalCount)> SearchAsync(string? keyword, int page, int size);
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(long id);
}