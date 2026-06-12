namespace Hospital.Domain.Entities;

/// <summary>账单明细，记录每项收费对应的业务单据</summary>
public class BillingItem : Entity
{
    private BillingItem() { } // For EF Core

    public BillingItem(long billingId, string itemType, string itemName, decimal amount)
    {
        BillingId = billingId;
        ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
        ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
        Amount = amount;
    }

    public long BillingId { get; private set; }
    public string ItemType { get; private set; } // Prescription / LabOrder / RadOrder
    public string ItemName { get; private set; }
    public decimal Amount { get; private set; }
}
