using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class BillingRepository : IBillingRepository
{
    private readonly List<Billing> _billings = new();

    public Task<Billing?> GetByIdAsync(long id)
        => Task.FromResult(_billings.FirstOrDefault(b => b.Id == id));

    public Task<List<Billing>> GetByPatientIdAsync(long patientId)
        => Task.FromResult(_billings.Where(b => b.PatientId == patientId).ToList());

    public Task<List<Billing>> GetByDateRangeAsync(DateTime from, DateTime to)
        => Task.FromResult(_billings.Where(b => b.CreatedAt >= from && b.CreatedAt <= to).ToList());

    public Task AddAsync(Billing billing)
    {
        billing.GetType().GetProperty("Id")?.SetValue(billing, _billings.Count + 1);
        _billings.Add(billing);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Billing billing)
    {
        var index = _billings.FindIndex(b => b.Id == billing.Id);
        if (index >= 0)
            _billings[index] = billing;
        return Task.CompletedTask;
    }
}
