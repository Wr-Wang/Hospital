using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Entities;

namespace Hospital.Application.Services;

public sealed class LabOrderApplicationService : ILabOrderApplicationService
{
    private readonly ILabOrderRepository _labOrderRepository;

    public LabOrderApplicationService(ILabOrderRepository labOrderRepository)
    {
        _labOrderRepository = labOrderRepository;
    }

    public async Task<List<LabOrderDto>> GetByEncounterAsync(long encounterId)
    {
        var list = await _labOrderRepository.GetByEncounterIdAsync(encounterId);
        return list.Select(o => new LabOrderDto(
            o.Id, o.EncounterId, o.ItemCode, o.ItemName, o.Status.ToString())).ToList();
    }

    public async Task<long> CreateAsync(long encounterId, CreateLabOrderDto dto)
    {
        var order = new LabOrder(encounterId, dto.ItemCode, dto.ItemName);
        await _labOrderRepository.AddAsync(order);
        return order.Id;
    }

    public async Task CancelAsync(long id)
    {
        var order = await _labOrderRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"检验申请不存在 (Id={id})");

        order.Cancel();
        await _labOrderRepository.UpdateAsync(order);
    }
}
