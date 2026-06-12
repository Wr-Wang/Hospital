using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class LabOrderRepository : ILabOrderRepository
{
    private readonly List<LabOrder> _orders = new();

    public Task<LabOrder?> GetByIdAsync(long id)
        => Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));

    public Task<List<LabOrder>> GetByEncounterIdAsync(long encounterId)
        => Task.FromResult(_orders.Where(o => o.EncounterId == encounterId).ToList());

    public Task AddAsync(LabOrder order)
    {
        order.GetType().GetProperty("Id")?.SetValue(order, _orders.Count + 1);
        _orders.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(LabOrder order)
    {
        return Task.CompletedTask;
    }
}
