namespace Hospital.Domain.ValueObjects;

public class PhoneNumber : IEquatable<PhoneNumber>
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));

        // Basic validation
        if (!IsValidPhoneNumber(value))
            throw new ArgumentException("Invalid phone number format", nameof(value));

        Value = value;
    }

    private static bool IsValidPhoneNumber(string value)
    {
        // Simple validation: digits, spaces, hyphens, parentheses
        return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\d\s\-\(\)]+$");
    }

    public bool Equals(PhoneNumber? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => Equals(obj as PhoneNumber);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static bool operator ==(PhoneNumber left, PhoneNumber right) => Equals(left, right);

    public static bool operator !=(PhoneNumber left, PhoneNumber right) => !Equals(left, right);
}