using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Domain.Entities;

namespace Hospital.Application.Repositories;

public interface IRegistrationRepository
{
    Task<Registration?> GetByIdAsync(long id);
    Task<List<Registration>> GetByPatientAsync(long patientId);
    Task<List<Registration>> GetByDoctorAsync(long doctorId, DateOnly? date);
    Task<int> GetNextQueueNumberAsync(long scheduleId, string slotName);
    Task AddAsync(Registration registration);
    Task UpdateAsync(Registration registration);
}
