using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface ILabOrderApplicationService
{
    Task<List<LabOrderDto>> GetByEncounterAsync(long encounterId);
    Task<long> CreateAsync(long encounterId, CreateLabOrderDto dto);
    Task CancelAsync(long id);
}
