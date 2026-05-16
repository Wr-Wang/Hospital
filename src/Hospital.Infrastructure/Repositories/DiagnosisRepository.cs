using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class DiagnosisRepository : IDiagnosisRepository
{
    private readonly List<Diagnosis> _diagnoses = new();

    public Task<List<Diagnosis>> GetByEncounterIdAsync(long encounterId)
        => Task.FromResult(_diagnoses.Where(d => d.EncounterId == encounterId).ToList());

    public Task AddAsync(Diagnosis diagnosis)
    {
        diagnosis.GetType().GetProperty("Id")?.SetValue(diagnosis, _diagnoses.Count + 1);
        _diagnoses.Add(diagnosis);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(long id)
    {
        var item = _diagnoses.FirstOrDefault(d => d.Id == id);
        if (item is not null)
            _diagnoses.Remove(item);
        return Task.CompletedTask;
    }
}
