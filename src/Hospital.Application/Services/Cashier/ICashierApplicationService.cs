using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Application.DTOs;

namespace Hospital.Application.Services;

public interface ICashierApplicationService
{
    Task<List<ChargeItemDto>> GetPendingItemsAsync(long patientId);
    Task PayAsync(PayRequestDto dto);
}
