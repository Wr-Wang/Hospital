using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class RadOrderRepository : IRadOrderRepository
{
    private readonly List<RadOrder> _orders = new();

    public Task<RadOrder?> GetByIdAsync(long id)
        => Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));

    public Task<List<RadOrder>> GetByEncounterIdAsync(long encounterId)
        => Task.FromResult(_orders.Where(o => o.EncounterId == encounterId).ToList());

    public Task AddAsync(RadOrder order)
    {
        order.GetType().GetProperty("Id")?.SetValue(order, _orders.Count + 1);
        _orders.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RadOrder order)
    {
        return Task.CompletedTask;
    }
}
