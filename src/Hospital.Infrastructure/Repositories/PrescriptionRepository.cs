using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly List<Prescription> _prescriptions = new();

    public Task<Prescription?> GetByIdAsync(long id)
        => Task.FromResult(_prescriptions.FirstOrDefault(p => p.Id == id));

    public Task<List<Prescription>> GetByEncounterIdAsync(long encounterId)
        => Task.FromResult(_prescriptions.Where(p => p.EncounterId == encounterId).ToList());

    public Task AddAsync(Prescription prescription)
    {
        prescription.GetType().GetProperty("Id")?.SetValue(prescription, _prescriptions.Count + 1);

        // Assign IDs to items
        var itemId = 1;
        foreach (var item in prescription.Items ?? Array.Empty<PrescriptionItem>())
        {
            item.GetType().GetProperty("Id")?.SetValue(item, itemId++);
        }

        _prescriptions.Add(prescription);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Prescription prescription)
    {
        return Task.CompletedTask;
    }
}
