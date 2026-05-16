using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Infrastructure.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly List<Registration> _registrations = new();

    public Task<Registration?> GetByIdAsync(long id)
        => Task.FromResult(_registrations.FirstOrDefault(r => r.Id == id));

    public Task<List<Registration>> GetByPatientAsync(long patientId)
        => Task.FromResult(_registrations
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.RegisterTime)
            .ToList());

    public Task<List<Registration>> GetByDoctorAsync(long doctorId, DateOnly? date)
    {
        var query = _registrations.Where(r => r.DoctorId == doctorId);
        if (date.HasValue)
            query = query.Where(r => DateOnly.FromDateTime(r.RegisterTime) == date.Value);
        return Task.FromResult(query.OrderByDescending(r => r.RegisterTime).ToList());
    }

    public Task<int> GetNextQueueNumberAsync(long scheduleId, string slotName)
    {
        var max = _registrations
            .Where(r => r.ScheduleId == scheduleId && r.SlotName == slotName)
            .Select(r => r.QueueNumber)
            .DefaultIfEmpty(0)
            .Max();
        return Task.FromResult(max + 1);
    }

    public Task AddAsync(Registration registration)
    {
        registration.GetType().GetProperty("Id")?.SetValue(registration, _registrations.Count + 1);
        _registrations.Add(registration);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Registration registration)
    {
        // 内存实现：引用保持不变，无需额外操作
        return Task.CompletedTask;
    }
}
