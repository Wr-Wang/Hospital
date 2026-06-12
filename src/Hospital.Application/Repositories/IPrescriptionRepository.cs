using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IPrescriptionRepository
{
    Task<Prescription?> GetByIdAsync(long id);
    Task<List<Prescription>> GetByEncounterIdAsync(long encounterId);
    Task AddAsync(Prescription prescription);
    Task UpdateAsync(Prescription prescription);
}
