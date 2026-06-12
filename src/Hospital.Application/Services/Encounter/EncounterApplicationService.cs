using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Enums;

namespace Hospital.Application.Services;

public sealed class EncounterApplicationService : IEncounterApplicationService
{
    private readonly IEncounterRepository _encounterRepository;
    private readonly IRegistrationRepository _registrationRepository;

    public EncounterApplicationService(
        IEncounterRepository encounterRepository,
        IRegistrationRepository registrationRepository)
    {
        _encounterRepository = encounterRepository;
        _registrationRepository = registrationRepository;
    }

    public async Task<List<EncounterQueueItemDto>> GetQueueAsync(long doctorId, string date)
    {
        var parsed = DateOnly.Parse(date);
        var encounters = await _encounterRepository.GetQueueAsync(doctorId, parsed);

        var dtos = new List<EncounterQueueItemDto>();
        foreach (var e in encounters)
        {
            var reg = await _registrationRepository.GetByIdAsync(e.RegistrationId);
            dtos.Add(new EncounterQueueItemDto(
                e.Id, e.PatientId, $"患者{e.PatientId}", string.Empty, string.Empty,
                reg?.QueueNumber ?? 0, reg?.SlotName ?? string.Empty,
                e.Status.ToString(), reg?.RegisterTime.ToString("HH:mm") ?? string.Empty));
        }

        return dtos
            .OrderBy(d => ExtractSlotHour(d.SlotName))
            .ThenBy(d => d.QueueNumber)
            .ToList();
    }

    /// <summary>从时段名称（如"8:00-9:00"）中提取起始小时，用于排序</summary>
    private static int ExtractSlotHour(string slotName)
    {
        if (string.IsNullOrWhiteSpace(slotName)) return 99;
        var hour = slotName.Split('-')[0].Trim();
        return int.TryParse(hour, out var h) ? h : 99;
    }

    public async Task StartConsultationAsync(long id)
    {
        var encounter = await _encounterRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"就诊记录不存在 (Id={id})");

        encounter.StartConsultation();
        await _encounterRepository.UpdateAsync(encounter);
    }

    public async Task CompleteConsultationAsync(long id)
    {
        var encounter = await _encounterRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"就诊记录不存在 (Id={id})");

        encounter.CompleteConsultation();
        await _encounterRepository.UpdateAsync(encounter);
    }
}
