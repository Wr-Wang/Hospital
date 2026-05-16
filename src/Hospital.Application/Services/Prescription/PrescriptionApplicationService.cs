using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Application.Services;

public sealed class PrescriptionApplicationService : IPrescriptionApplicationService
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public PrescriptionApplicationService(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<List<PrescriptionDto>> GetByEncounterAsync(long encounterId)
    {
        var list = await _prescriptionRepository.GetByEncounterIdAsync(encounterId);
        return list.Select(MapToDto).ToList();
    }

    public async Task<long> CreateAsync(long encounterId, long doctorId, CreatePrescriptionDto dto)
    {
        var prescription = new Prescription(encounterId, doctorId);

        foreach (var item in dto.Items)
        {
            var itemEntity = new PrescriptionItem(
                0, item.DrugName, item.Spec, item.Form,
                item.Freq, item.Dosage, item.Duration, item.Qty, item.Note);
            prescription.AddItem(itemEntity);
        }

        await _prescriptionRepository.AddAsync(prescription);
        return prescription.Id;
    }

    public async Task VoidAsync(long id)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"处方不存在 (Id={id})");

        prescription.Void();
        await _prescriptionRepository.UpdateAsync(prescription);
    }

    private static PrescriptionDto MapToDto(Prescription p)
    {
        return new PrescriptionDto(
            p.Id, p.EncounterId, p.DoctorId, p.Status.ToString(),
            p.Items.Select(i => new PrescriptionItemDto(
                i.Id, i.DrugName, i.Spec, i.Form, i.Freq,
                i.Dosage, i.Duration, i.Qty, i.Note)).ToList());
    }
}
