using System.Threading.Tasks;
using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IEncounterRepository
{
    Task<Encounter?> GetByIdAsync(long id);
    Task<Encounter?> GetByRegistrationIdAsync(long registrationId);
    Task AddAsync(Encounter encounter);
    Task UpdateAsync(Encounter encounter);
}
