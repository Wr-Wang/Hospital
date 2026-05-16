namespace Hospital.Domain.Entities;

/// <summary>药品库存实体，记录每种药品的批号、效期、库存数量</summary>
public class DrugInventory : Entity
{
    private DrugInventory() { } // For EF Core

    public DrugInventory(string drugCode, string drugName, string spec, string batchNo,
        DateTime expiryDate, int totalQuantity)
    {
        DrugCode = drugCode ?? throw new ArgumentNullException(nameof(drugCode));
        DrugName = drugName ?? throw new ArgumentNullException(nameof(drugName));
        Spec = spec ?? throw new ArgumentNullException(nameof(spec));
        BatchNo = batchNo ?? throw new ArgumentNullException(nameof(batchNo));
        ExpiryDate = expiryDate;
        TotalQuantity = totalQuantity;
        AvailableQuantity = totalQuantity;
    }

    public string DrugCode { get; private set; }
    public string DrugName { get; private set; }
    public string Spec { get; private set; }
    public string BatchNo { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public int TotalQuantity { get; private set; }
    public int AvailableQuantity { get; private set; }

    /// <summary>扣减库存，返回是否足够</summary>
    public bool Deduct(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("扣减数量必须大于 0", nameof(quantity));

        if (AvailableQuantity < quantity)
            return false;

        AvailableQuantity -= quantity;
        return true;
    }

    /// <summary>回冲库存（退药时使用）</summary>
    public void Restore(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("回冲数量必须大于 0", nameof(quantity));

        AvailableQuantity += quantity;
        if (AvailableQuantity > TotalQuantity)
            AvailableQuantity = TotalQuantity;
    }

    /// <summary>是否已过期</summary>
    public bool IsExpired => DateTime.Now > ExpiryDate;
}
