using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities;

/// <summary>处方聚合根</summary>
public class Prescription : AggregateRoot
{
    private readonly List<PrescriptionItem> _items = new();

    // EF Core
    private Prescription() { }

    public Prescription(long encounterId, long doctorId)
    {
        EncounterId = encounterId;
        DoctorId = doctorId;
        Status = PrescriptionStatus.待缴费;
    }

    public long EncounterId { get; private set; }
    public long DoctorId { get; private set; }
    public PrescriptionStatus Status { get; private set; }
    public IReadOnlyList<PrescriptionItem> Items => _items.AsReadOnly();

    public void AddItem(PrescriptionItem item)
    {
        _items.Add(item);
    }

    /// <summary>缴费</summary>
    public void Pay()
    {
        if (Status != PrescriptionStatus.待缴费)
            throw new InvalidOperationException("仅待缴费状态的处方可以缴费");

        Status = PrescriptionStatus.已缴费;
    }

    /// <summary>发药</summary>
    public void Dispense()
    {
        if (Status != PrescriptionStatus.已缴费)
            throw new InvalidOperationException("仅已缴费状态的处方可以发药");

        Status = PrescriptionStatus.已发药;
    }

    /// <summary>作废处方</summary>
    public void Void()
    {
        if (Status == PrescriptionStatus.已发药)
            throw new InvalidOperationException("已发药的处方不能作废");

        Status = PrescriptionStatus.已退药;
    }
}
