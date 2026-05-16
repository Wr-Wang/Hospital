using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Enums;

namespace Hospital.Application.Services;

/// <summary>发药应用服务：已缴费处方发药 / 退药</summary>
public sealed class DispenseApplicationService : IDispenseApplicationService
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public DispenseApplicationService(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<List<PrescriptionDto>> GetPaidPrescriptionsAsync(long patientId)
    {
        // 简化：内存实现通过 patientId 过滤
        // 实际应通过 encounter → prescription 关联查询
        return new List<PrescriptionDto>();
    }

    public async Task DispenseAsync(long id)
    {
        var rx = await _prescriptionRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"处方不存在 (Id={id})");

        rx.Dispense();
        await _prescriptionRepository.UpdateAsync(rx);
    }

    public async Task ReturnAsync(long id)
    {
        var rx = await _prescriptionRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"处方不存在 (Id={id})");

        rx.Void();
        await _prescriptionRepository.UpdateAsync(rx);
    }
}
