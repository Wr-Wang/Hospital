using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IEncounterApplicationService
{
    Task<List<EncounterQueueItemDto>> GetQueueAsync(long doctorId, string date);
    Task StartConsultationAsync(long id);
    Task CompleteConsultationAsync(long id);
}
