using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IEncounterRepository
{
    Task<Encounter?> GetByIdAsync(long id);
    Task<Encounter?> GetByRegistrationIdAsync(long registrationId);
    Task<List<Encounter>> GetByDoctorAsync(long doctorId);
    Task<List<Encounter>> GetByPatientAsync(long patientId);
    Task<List<Encounter>> GetByDateAsync(long doctorId, DateOnly date);
    Task<List<Encounter>> GetQueueAsync(long doctorId, DateOnly date);
    Task AddAsync(Encounter encounter);
    Task UpdateAsync(Encounter encounter);
}
