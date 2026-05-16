using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IPrescriptionApplicationService
{
    Task<List<PrescriptionDto>> GetByEncounterAsync(long encounterId);
    Task<long> CreateAsync(long encounterId, long doctorId, CreatePrescriptionDto dto);
    Task VoidAsync(long id);
}
