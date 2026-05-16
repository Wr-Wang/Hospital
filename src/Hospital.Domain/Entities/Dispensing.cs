using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities;

/// <summary>发药记录聚合，记录一次发药/退药操作</summary>
public class Dispensing : Entity
{
    private readonly List<DispenseItem> _items = new();

    private Dispensing() { } // For EF Core

    public Dispensing(long prescriptionId, long dispensedBy)
    {
        PrescriptionId = prescriptionId;
        DispensedBy = dispensedBy;
        Status = DispenseStatus.待审核;
        OperatedAt = DateTime.Now;
    }

    public long PrescriptionId { get; private set; }
    public long DispensedBy { get; private set; }
    public DispenseStatus Status { get; private set; }
    public DateTime OperatedAt { get; private set; }
    public string? Remark { get; private set; }

    public IReadOnlyCollection<DispenseItem> Items => _items.AsReadOnly();

    public void AddItem(DispenseItem item) => _items.Add(item);

    public void Approve()
    {
        if (Status != DispenseStatus.待审核)
            throw new InvalidOperationException("只有待审核状态才能审核通过");
        Status = DispenseStatus.已审核;
    }

    public void Dispense(string? remark = null)
    {
        if (Status != DispenseStatus.已审核)
            throw new InvalidOperationException("只有已审核状态才能发药");
        Status = DispenseStatus.已发药;
        Remark = remark;
    }

    public void Return(string? remark = null)
    {
        if (Status != DispenseStatus.已发药)
            throw new InvalidOperationException("只有已发药状态才能退药");
        Status = DispenseStatus.已退药;
        Remark = remark;
    }
}
