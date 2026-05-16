using Hospital.Application.Repositories;
using Hospital.Domain.Aggregates.Patient;

namespace Hospital.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
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

    public Task<Patient?> GetByIdCardAsync(string idCard)
    {
        var patient = _patients.FirstOrDefault(p => p.IdCard?.Number == idCard);
        return Task.FromResult(patient);
    }

    public Task<List<Patient>> GetSuspectDuplicatesAsync(string name, string? phone)
    {
        var matches = _patients.Where(p =>
            p.Name.Contains(name, StringComparison.OrdinalIgnoreCase) &&
            (phone == null || p.Phone?.Value == phone))
            .Take(10)
            .ToList();
        return Task.FromResult(matches);
    }

    public Task<(List<Patient> Items, int TotalCount)> SearchAsync(string? keyword, int page, int size)
    {
        var query = _patients.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p =>
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                (p.IdCard?.Number.Contains(keyword) ?? false) ||
                (p.Phone?.Value.Contains(keyword) ?? false) ||
                p.PatientNo.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        return Task.FromResult((items, totalCount));
    }

    public Task AddAsync(Patient patient)
    {
        patient.GetType().GetProperty("Id")?.SetValue(patient, _patients.Count + 1);
        _patients.Add(patient);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Patient patient)
    {
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
