namespace Hospital.Domain.Entities;

/// <summary>发药明细，记录每种药品的发药数量</summary>
public class DispenseItem : Entity
{
    private DispenseItem() { } // For EF Core

    public DispenseItem(long dispensingId, long drugInventoryId, string drugName, string spec, int quantity)
    {
        DispensingId = dispensingId;
        DrugInventoryId = drugInventoryId;
        DrugName = drugName ?? throw new ArgumentNullException(nameof(drugName));
        Spec = spec ?? throw new ArgumentNullException(nameof(spec));
        Quantity = quantity;
    }

    public long DispensingId { get; private set; }
    public long DrugInventoryId { get; private set; }
    public string DrugName { get; private set; }
    public string Spec { get; private set; }
    public int Quantity { get; private set; }
}
