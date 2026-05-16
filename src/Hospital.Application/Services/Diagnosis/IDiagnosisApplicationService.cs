using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IDiagnosisApplicationService
{
    Task<List<DiagnosisDto>> GetByEncounterAsync(long encounterId);
    Task<long> AddAsync(long encounterId, CreateDiagnosisDto dto);
    Task RemoveAsync(long id);
}
