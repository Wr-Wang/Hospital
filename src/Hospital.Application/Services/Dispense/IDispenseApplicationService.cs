using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IDispenseApplicationService
{
    Task<List<PrescriptionDto>> GetPaidPrescriptionsAsync(long patientId);
    Task DispenseAsync(long id);
    Task ReturnAsync(long id);
}
