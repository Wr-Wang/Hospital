using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IBillingRepository
{
    Task<Billing?> GetByIdAsync(long id);
    Task<List<Billing>> GetByPatientIdAsync(long patientId);
    Task<List<Billing>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task AddAsync(Billing billing);
    Task UpdateAsync(Billing billing);
}
