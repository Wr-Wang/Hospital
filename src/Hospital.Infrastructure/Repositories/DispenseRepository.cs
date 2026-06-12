using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class DispenseRepository : IDispenseRepository
{
    private readonly List<Dispensing> _dispensings = new();

    public Task<Dispensing?> GetByIdAsync(long id)
        => Task.FromResult(_dispensings.FirstOrDefault(d => d.Id == id));

    public Task<List<Dispensing>> GetByPrescriptionIdAsync(long prescriptionId)
        => Task.FromResult(_dispensings.Where(d => d.PrescriptionId == prescriptionId).ToList());

    public Task AddAsync(Dispensing dispensing)
    {
        dispensing.GetType().GetProperty("Id")?.SetValue(dispensing, _dispensings.Count + 1);
        _dispensings.Add(dispensing);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Dispensing dispensing)
    {
        var index = _dispensings.FindIndex(d => d.Id == dispensing.Id);
        if (index >= 0)
            _dispensings[index] = dispensing;
        return Task.CompletedTask;
    }
}
