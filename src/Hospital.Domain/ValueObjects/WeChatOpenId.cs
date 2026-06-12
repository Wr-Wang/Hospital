namespace Hospital.Domain.ValueObjects;

public sealed record WeChatOpenId
{
    public WeChatOpenId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("OpenId 不能为空", nameof(value));
        Value = value.Trim();
    }

    public string Value { get; }
}
