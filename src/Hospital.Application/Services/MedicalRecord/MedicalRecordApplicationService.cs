using System;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Application.Services;

public sealed class MedicalRecordApplicationService : IMedicalRecordApplicationService
{
    private readonly IMedicalRecordRepository _recordRepository;

    public MedicalRecordApplicationService(IMedicalRecordRepository recordRepository)
    {
        _recordRepository = recordRepository;
    }

    public async Task<MedicalRecordDto?> GetByEncounterAsync(long encounterId)
    {
        var record = await _recordRepository.GetByEncounterIdAsync(encounterId);
        if (record is null) return null;

        return new MedicalRecordDto(
            record.Id, record.EncounterId,
            record.ChiefComplaint, record.PresentIllness,
            record.PastHistory, record.PhysicalExam,
            record.Status.ToString(), record.Version);
    }

    public async Task SaveAsync(long encounterId, SaveMedicalRecordDto dto)
    {
        var existing = await _recordRepository.GetByEncounterIdAsync(encounterId);

        if (existing is null)
        {
            var record = new MedicalRecord(encounterId, dto.ChiefComplaint);
            record.SaveDraft(dto.ChiefComplaint, dto.PresentIllness, dto.PastHistory, dto.PhysicalExam);

            if (dto.IsSubmit)
                record.Submit();

            await _recordRepository.AddAsync(record);
        }
        else
        {
            if (existing.Status == Domain.Enums.RecordStatus.终稿)
                existing.Update(dto.ChiefComplaint, dto.PresentIllness, dto.PastHistory, dto.PhysicalExam);
            else
                existing.SaveDraft(dto.ChiefComplaint, dto.PresentIllness, dto.PastHistory, dto.PhysicalExam);

            if (dto.IsSubmit)
                existing.Submit();

            await _recordRepository.UpdateAsync(existing);
        }
    }
}
