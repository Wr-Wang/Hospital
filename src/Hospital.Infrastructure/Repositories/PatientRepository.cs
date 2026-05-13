using Hospital.Application.Repositories;
using Hospital.Domain.Aggregates.Patient;

namespace Hospital.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    // For now, mock implementation. Later integrate with EF Core
    private readonly List<Patient> _patients = new();

    public Task<Patient?> GetByIdAsync(long id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(patient);
    }

    public Task<Patient?> GetByPatientNoAsync(string patientNo)
    {
        var patient = _patients.FirstOrDefault(p => p.PatientNo == patientNo);
        return Task.FromResult(patient);
    }

    public Task AddAsync(Patient patient)
    {
        // Simulate ID assignment
        patient.GetType().GetProperty("Id")?.SetValue(patient, _patients.Count + 1);
        _patients.Add(patient);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Patient patient)
    {
        // Mock update
        return Task.CompletedTask;
    }

    public Task DeleteAsync(long id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient is not null)
            _patients.Remove(patient);
        return Task.CompletedTask;
    }
}