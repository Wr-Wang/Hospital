using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IRadOrderRepository
{
    Task<RadOrder?> GetByIdAsync(long id);
    Task<List<RadOrder>> GetByEncounterIdAsync(long encounterId);
    Task<List<RadOrder>> GetByPatientAsync(long patientId);
    Task AddAsync(RadOrder order);
    Task UpdateAsync(RadOrder order);
}
