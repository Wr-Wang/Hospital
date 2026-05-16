using System;
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

    public Task<List<Encounter>> GetByDoctorAsync(long doctorId)
        => Task.FromResult(_encounters.Where(e => e.DoctorId == doctorId).ToList());

    public Task<List<Encounter>> GetByPatientAsync(long patientId)
        => Task.FromResult(_encounters.Where(e => e.PatientId == patientId).ToList());

    public Task<List<Encounter>> GetByDateAsync(long doctorId, DateOnly date)
    {
        // Filter encounters belonging to the given doctor and approximate date.
        // In a real EF Core implementation this would use a proper date column.
        var dayStart = date.DayNumber * 1000L;
        var dayEnd = dayStart + 999;
        return Task.FromResult(_encounters
            .Where(e => e.DoctorId == doctorId && e.Id >= dayStart && e.Id <= dayEnd)
            .ToList());
    }

    public Task<List<Encounter>> GetQueueAsync(long doctorId, DateOnly date)
    {
        return Task.FromResult(_encounters
            .Where(e => e.DoctorId == doctorId)
            .OrderBy(e => e.Id)
            .ToList());
    }

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
