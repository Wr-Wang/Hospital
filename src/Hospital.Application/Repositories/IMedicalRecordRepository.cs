using System.Threading.Tasks;
using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IMedicalRecordRepository
{
    Task<MedicalRecord?> GetByEncounterIdAsync(long encounterId);
    Task AddAsync(MedicalRecord record);
    Task UpdateAsync(MedicalRecord record);
}
