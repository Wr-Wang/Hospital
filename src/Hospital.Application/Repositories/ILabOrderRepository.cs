using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface ILabOrderRepository
{
    Task<LabOrder?> GetByIdAsync(long id);
    Task<List<LabOrder>> GetByEncounterIdAsync(long encounterId);
    Task AddAsync(LabOrder order);
    Task UpdateAsync(LabOrder order);
}
