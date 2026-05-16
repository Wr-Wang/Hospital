namespace Hospital.Domain.ValueObjects;

public class IdCard : IEquatable<IdCard>
{
    public string Number { get; }

    public IdCard(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("身份证号不能为空", nameof(number));

        if (!IsValidIdCard(number))
            throw new ArgumentException("身份证号格式不正确（应为 18 位）", nameof(number));

        Number = number;
    }

    private static bool IsValidIdCard(string number)
    {
        // Basic validation for Chinese ID card
        return System.Text.RegularExpressions.Regex.IsMatch(number, @"^\d{17}[\dXx]$");
    }

    public bool Equals(IdCard? other) => other is not null && Number == other.Number;

    public override bool Equals(object? obj) => Equals(obj as IdCard);

    public override int GetHashCode() => Number.GetHashCode();

    public override string ToString() => Number;

    public static bool operator ==(IdCard left, IdCard right) => Equals(left, right);

    public static bool operator !=(IdCard left, IdCard right) => !Equals(left, right);
}