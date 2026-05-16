using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IDispenseRepository
{
    Task<Dispensing?> GetByIdAsync(long id);
    Task<List<Dispensing>> GetByPrescriptionIdAsync(long prescriptionId);
    Task AddAsync(Dispensing dispensing);
    Task UpdateAsync(Dispensing dispensing);
}
