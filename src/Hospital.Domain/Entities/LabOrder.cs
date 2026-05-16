using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities;

/// <summary>检验申请实体</summary>
public class LabOrder : Entity
{
    // EF Core
    private LabOrder() { }

    public LabOrder(long encounterId, string itemCode, string itemName)
    {
        EncounterId = encounterId;
        ItemCode = itemCode;
        ItemName = itemName;
        Status = OrderStatus.已开立;
    }

    public long EncounterId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }

    /// <summary>取消申请</summary>
    public void Cancel()
    {
        if (Status == OrderStatus.已执行 || Status == OrderStatus.已报告)
            throw new InvalidOperationException("已执行或已报告的申请不能取消");

        Status = OrderStatus.已取消;
    }
}
