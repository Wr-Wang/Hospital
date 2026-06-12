using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;
using Hospital.Domain.Enums;

namespace Hospital.Application.Services;

public sealed class DiagnosisApplicationService : IDiagnosisApplicationService
{
    private readonly IDiagnosisRepository _diagnosisRepository;

    public DiagnosisApplicationService(IDiagnosisRepository diagnosisRepository)
    {
        _diagnosisRepository = diagnosisRepository;
    }

    public async Task<List<DiagnosisDto>> GetByEncounterAsync(long encounterId)
    {
        var list = await _diagnosisRepository.GetByEncounterIdAsync(encounterId);
        return list.Select(d => new DiagnosisDto(
            d.Id, d.EncounterId, d.DiagnosisType.ToString(),
            d.IcdCode, d.Description, d.IsPrimary)).ToList();
    }

    public async Task<long> AddAsync(long encounterId, CreateDiagnosisDto dto)
    {
        var type = Enum.Parse<DiagnosisType>(dto.DiagnosisType);
        var diagnosis = new Diagnosis(encounterId, type, dto.IcdCode, dto.Description, dto.IsPrimary);
        await _diagnosisRepository.AddAsync(diagnosis);
        return diagnosis.Id;
    }

    public async Task RemoveAsync(long id)
    {
        await _diagnosisRepository.RemoveAsync(id);
    }
}
