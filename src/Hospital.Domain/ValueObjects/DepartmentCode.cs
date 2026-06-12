namespace Hospital.Domain.ValueObjects;

public sealed record DepartmentCode
{
    public string Value { get; }

    public DepartmentCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("科室编码不能为空", nameof(value));

        if (value.Length is < 1 or > 20)
            throw new ArgumentException("科室编码长度应在 1-20 个字符之间", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(DepartmentCode code) => code.Value;
}
