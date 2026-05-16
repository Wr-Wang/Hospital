using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class MedicalRecordRepository : IMedicalRecordRepository
{
    private readonly List<MedicalRecord> _records = new();

    public Task<MedicalRecord?> GetByEncounterIdAsync(long encounterId)
        => Task.FromResult(_records.FirstOrDefault(r => r.EncounterId == encounterId));

    public Task AddAsync(MedicalRecord record)
    {
        record.GetType().GetProperty("Id")?.SetValue(record, _records.Count + 1);
        _records.Add(record);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MedicalRecord record)
    {
        return Task.CompletedTask;
    }
}
