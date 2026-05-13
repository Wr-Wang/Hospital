using Hospital.Domain.Aggregates.Patient;

namespace Hospital.Application.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(long id);
    Task<Patient?> GetByPatientNoAsync(string patientNo);
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(long id);
}