using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IRegistrationApplicationService
{
    Task<RegistrationDto?> GetByIdAsync(long id);
    Task<List<RegistrationDto>> GetByPatientAsync(long patientId);
    Task<List<RegistrationDto>> GetByDoctorAsync(long doctorId, string? date);
    Task<long> RegisterAsync(CreateRegistrationDto dto);
    Task VoidAsync(long id);
}
