using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities;

/// <summary>支付记录，记录一次支付操作</summary>
public class Payment : Entity
{
    private Payment() { } // For EF Core

    public Payment(PaymentMethod method, decimal amount, string? remark)
    {
        Method = method;
        Amount = amount;
        Remark = remark;
        PaidAt = DateTime.Now;
    }

    public PaymentMethod Method { get; private set; }
    public decimal Amount { get; private set; }
    public string? Remark { get; private set; }
    public DateTime PaidAt { get; private set; }
}
