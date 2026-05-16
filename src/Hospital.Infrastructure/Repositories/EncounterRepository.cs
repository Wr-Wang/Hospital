using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class EncounterRepository : IEncounterRepository
{
    private readonly List<Encounter> _encounters = new();

    public Task<Encounter?> GetByIdAsync(long id)
        => Task.FromResult(_encounters.FirstOrDefault(e => e.Id == id));

    public Task<Encounter?> GetByRegistrationIdAsync(long registrationId)
        => Task.FromResult(_encounters.FirstOrDefault(e => e.RegistrationId == registrationId));

    public Task AddAsync(Encounter encounter)
    {
        encounter.GetType().GetProperty("Id")?.SetValue(encounter, _encounters.Count + 1);
        _encounters.Add(encounter);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Encounter encounter)
    {
        return Task.CompletedTask;
    }
}
