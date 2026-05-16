namespace Hospital.Domain.ValueObjects;

public sealed record CampusCode
{
    public string Value { get; }

    public CampusCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("院区编码不能为空", nameof(value));

        if (value.Length is < 2 or > 20)
            throw new ArgumentException("院区编码长度应在 2-20 个字符之间", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(CampusCode code) => code.Value;
}
