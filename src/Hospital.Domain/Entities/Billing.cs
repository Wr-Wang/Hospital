using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities;

/// <summary>账单聚合，记录一次缴费事务</summary>
public class Billing : Entity
{
    private readonly List<BillingItem> _items = new();
    private readonly List<Payment> _payments = new();

    private Billing() { } // For EF Core

    public Billing(long patientId, string patientName, decimal totalAmount)
    {
        PatientId = patientId;
        PatientName = patientName ?? throw new ArgumentNullException(nameof(patientName));
        TotalAmount = totalAmount;
        Status = BillingStatus.待缴;
        CreatedAt = DateTime.Now;
    }

    public long PatientId { get; private set; }
    public string PatientName { get; private set; }
    public decimal TotalAmount { get; private set; }
    public BillingStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public IReadOnlyCollection<BillingItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    public void AddItem(BillingItem item)
    {
        _items.Add(item);
        TotalAmount += item.Amount;
    }

    public void Pay(PaymentMethod method, decimal amount, string? remark = null)
    {
        if (Status != BillingStatus.待缴)
            throw new InvalidOperationException("账单不是待缴状态，无法缴费");

        var payment = new Payment(method, amount, remark);
        _payments.Add(payment);
        PaidAt = DateTime.Now;
        Status = BillingStatus.已缴;
    }

    public void Refund()
    {
        if (Status != BillingStatus.已缴)
            throw new InvalidOperationException("账单不是已缴状态，无法退费");

        Status = BillingStatus.已退;
    }
}
