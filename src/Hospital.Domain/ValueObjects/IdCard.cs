namespace Hospital.Domain.ValueObjects;

public class IdCard : IEquatable<IdCard>
{
    public string Number { get; }

    public IdCard(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("ID card number cannot be empty", nameof(number));

        if (!IsValidIdCard(number))
            throw new ArgumentException("Invalid ID card format", nameof(number));

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