using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IDiagnosisRepository
{
    Task<List<Diagnosis>> GetByEncounterIdAsync(long encounterId);
    Task AddAsync(Diagnosis diagnosis);
    Task RemoveAsync(long id);
}
