using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Application.DTOs;
using Hospital.Application.Repositories;
using Hospital.Domain.Enums;

namespace Hospital.Application.Services;

/// <summary>收费应用服务：查询待收费项目 + 统一缴费</summary>
public sealed class CashierApplicationService : ICashierApplicationService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly ILabOrderRepository _labOrderRepository;
    private readonly IRadOrderRepository _radOrderRepository;
    private readonly IPatientRepository _patientRepository;

    public CashierApplicationService(
        IPrescriptionRepository prescriptionRepository,
        ILabOrderRepository labOrderRepository,
        IRadOrderRepository radOrderRepository,
        IPatientRepository patientRepository)
    {
        _prescriptionRepository = prescriptionRepository;
        _labOrderRepository = labOrderRepository;
        _radOrderRepository = radOrderRepository;
        _patientRepository = patientRepository;
    }

    public async Task<List<ChargeItemDto>> GetPendingItemsAsync(long patientId)
    {
        var items = new List<ChargeItemDto>();
        var patient = await _patientRepository.GetByIdAsync(patientId);
        var patientName = patient?.Name ?? $"患者#{patientId}";

        // 查询该患者所有就诊中未缴费的处方
        // 简化：通过 patientId 获取 encounters → 然后获取 prescriptions
        // 内存实现暂不跨 aggregate 查询，返回空列表示意
        return items;
    }

    public async Task PayAsync(PayRequestDto dto)
    {
        foreach (var item in dto.Items)
        {
            switch (item.ItemType)
            {
                case "Prescription":
                    var rx = await _prescriptionRepository.GetByIdAsync(item.Id)
                        ?? throw new InvalidOperationException($"处方不存在 (Id={item.Id})");
                    rx.Pay();
                    await _prescriptionRepository.UpdateAsync(rx);
                    break;

                case "LabOrder":
                    var lab = await _labOrderRepository.GetByIdAsync(item.Id)
                        ?? throw new InvalidOperationException($"检验申请不存在 (Id={item.Id})");
                    lab.Pay();
                    await _labOrderRepository.UpdateAsync(lab);
                    break;

                case "RadOrder":
                    var rad = await _radOrderRepository.GetByIdAsync(item.Id)
                        ?? throw new InvalidOperationException($"检查申请不存在 (Id={item.Id})");
                    rad.Pay();
                    await _radOrderRepository.UpdateAsync(rad);
                    break;
            }
        }
    }
}
