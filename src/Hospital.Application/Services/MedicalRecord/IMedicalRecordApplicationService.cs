using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface IMedicalRecordApplicationService
{
    Task<MedicalRecordDto?> GetByEncounterAsync(long encounterId);
    Task SaveAsync(long encounterId, SaveMedicalRecordDto dto);
}
