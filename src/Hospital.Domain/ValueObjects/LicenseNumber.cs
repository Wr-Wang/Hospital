using System.Text.RegularExpressions;

namespace Hospital.Domain.ValueObjects;

public sealed record LicenseNumber
{
    public string Value { get; }

    public LicenseNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("执业资质编号不能为空", nameof(value));

        if (!Regex.IsMatch(value, @"^\d{15,20}$"))
            throw new ArgumentException("执业资质编号格式不正确（应为15-20位数字）", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(LicenseNumber license) => license.Value;
}
